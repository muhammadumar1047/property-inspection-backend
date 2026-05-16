using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PropertyInspection.API.Extensions;
using PropertyInspection.Application.IServices;
using PropertyInspection.Core.Enums;
using PropertyInspection.Core.Interfaces.UnitOfWork;
using PropertyInspection.Shared;
using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class S3Controller : ControllerBase
    {
        private readonly IS3Service _s3Service;
        private readonly IOptions<AwsSettings> _awsSettings;
        private readonly IAmazonS3 _s3Client;
        private readonly ITenantAgencyResolver _tenantAgencyResolver;
        private readonly IUnitOfWork _unitOfWork;

        public S3Controller(
            IS3Service s3Service,
            IOptions<AwsSettings> awsSettings,
            IAmazonS3 s3Client,
            ITenantAgencyResolver tenantAgencyResolver,
            IUnitOfWork unitOfWork)
        {
            _s3Service = s3Service;
            _awsSettings = awsSettings;
            _s3Client = s3Client;
            _tenantAgencyResolver = tenantAgencyResolver;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("generate-upload-url")]
        public async Task<ActionResult<ApiResponse<object>>> GenerateUploadUrl(
            [FromQuery] string? agencyId,
            [FromQuery] string propertyId,
            [FromQuery] string inspectionId,
            [FromQuery] string mediaType,
            [FromQuery] string contentType = "image/jpeg")
        {
            if (!Guid.TryParse(propertyId, out var propertyGuid) ||
                !Guid.TryParse(inspectionId, out var inspectionGuid))
            {
                return this.ToActionResult(new ServiceResponse<object>
                {
                    Success = false,
                    Message = "Invalid request data",
                    ErrorCode = ServiceErrorCodes.InvalidRequest
                });
            }

            Guid? requestedAgencyId = null;
            if (Guid.TryParse(agencyId, out var agencyGuid))
            {
                requestedAgencyId = agencyGuid;
            }

            var resolvedAgencyId = _tenantAgencyResolver.ResolveAgencyId(requestedAgencyId);
            var normalizedMediaType = NormalizeMediaType(mediaType);
            if (string.IsNullOrWhiteSpace(normalizedMediaType))
            {
                return this.ToActionResult(new ServiceResponse<object>
                {
                    Success = false,
                    Message = "Invalid request data",
                    ErrorCode = ServiceErrorCodes.InvalidRequest
                });
            }

            var validation = await ValidateInspectionAsync(resolvedAgencyId, propertyGuid, inspectionGuid);
            if (!validation.Success)
            {
                return this.ToActionResult(new ServiceResponse<object>
                {
                    Success = false,
                    Message = validation.Message,
                    ErrorCode = validation.ErrorCode
                });
            }

            var fileId = Guid.NewGuid().ToString("N");
            var extension = GetFileExtension(contentType);
            var fileName = string.IsNullOrWhiteSpace(extension) ? fileId : $"{fileId}{extension}";

            var folderPath = BuildFolderPath(resolvedAgencyId, propertyGuid, inspectionGuid, normalizedMediaType);
            var key = $"{folderPath}/{fileName}";

            var uploadUrl = GeneratePresignedUploadUrl(key, contentType);
            var fileUrl = BuildFileUrl(key);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Record retrieved successfully",
                Data = new
                {
                    uploadUrl,
                    fileUrl,
                    fileKey = key,
                    photoId = fileId
                }
            });
        }

        [HttpPost("generate-upload-urls")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<UploadFileResponse>>>> GenerateUploadUrls(
            [FromBody] GenerateUploadUrlsRequest request)
        {
            if (!ModelState.IsValid || request.Files.Count == 0)
            {
                return this.ToActionResult(new ServiceResponse<IReadOnlyList<UploadFileResponse>>
                {
                    Success = false,
                    Message = "Invalid request data",
                    ErrorCode = ServiceErrorCodes.InvalidRequest
                });
            }

            var resolvedAgencyId = _tenantAgencyResolver.ResolveAgencyId(request.AgencyId);
            var propertyId = request.PropertyId!.Value;
            var inspectionId = request.InspectionId!.Value;

            var validation = await ValidateInspectionAsync(resolvedAgencyId, propertyId, inspectionId);
            if (!validation.Success)
            {
                return this.ToActionResult(new ServiceResponse<IReadOnlyList<UploadFileResponse>>
                {
                    Success = false,
                    Message = validation.Message,
                    ErrorCode = validation.ErrorCode
                });
            }

            if (request.Files.Count > 20)
            {
                return this.ToActionResult(new ServiceResponse<IReadOnlyList<UploadFileResponse>>
                {
                    Success = false,
                    Message = "Too many files in a single request",
                    ErrorCode = ServiceErrorCodes.InvalidRequest
                });
            }

            var mediaFolder = GetMediaFolder(request.MediaType);
            var folderPath = BuildFolderPath(resolvedAgencyId, propertyId, inspectionId, mediaFolder);

            var responses = new List<UploadFileResponse>();
            foreach (var file in request.Files)
            {
                if (string.IsNullOrWhiteSpace(file.ContentType))
                {
                    return this.ToActionResult(new ServiceResponse<IReadOnlyList<UploadFileResponse>>
                    {
                        Success = false,
                        Message = "Invalid request data",
                        ErrorCode = ServiceErrorCodes.InvalidRequest
                    });
                }

                var fileId = Guid.NewGuid().ToString("N");
                var extension = GetFileExtension(file.ContentType);
                var fileName = string.IsNullOrWhiteSpace(extension) ? fileId : $"{fileId}{extension}";
                var key = $"{folderPath}/{fileName}";

                var uploadUrl = GeneratePresignedUploadUrl(key, file.ContentType);
                var fileUrl = BuildFileUrl(key);

                responses.Add(new UploadFileResponse
                {
                    UploadUrl = uploadUrl,
                    FileUrl = fileUrl,
                    FileKey = key,
                    ContentType = file.ContentType,
                    OriginalFileName = file.OriginalFileName
                });
            }

            return Ok(new ApiResponse<IReadOnlyList<UploadFileResponse>>
            {
                Success = true,
                Message = "Record retrieved successfully",
                Data = responses
            });
        }

        [HttpGet("generate-download-url")]
        public async Task<ActionResult<ApiResponse<object>>> GenerateDownloadUrl([FromQuery] string folder, [FromQuery] string fileName)
        {
            var result = await _s3Service.GeneratePresignedDownloadUrlAsync(folder, fileName);
            if (!result.Success)
            {
                return this.ToActionResult(new ServiceResponse<object>
                {
                    Success = false,
                    Message = result.Message,
                    ErrorCode = result.ErrorCode
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = result.Message,
                Data = new { downloadUrl = result.Data }
            });
        }

        [HttpDelete("delete-file")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteFile([FromQuery] string folder, [FromQuery] string fileName)
        {
            var result = await _s3Service.DeleteFileAsync(folder, fileName);
            return this.ToActionResult(result);
        }

        [HttpGet("file-exists")]
        public async Task<ActionResult<ApiResponse<bool>>> FileExists([FromQuery] string folder, [FromQuery] string fileName)
        {
            var result = await _s3Service.FileExistsAsync(folder, fileName);
            return this.ToActionResult(result);
        }

        private async Task<ServiceResponse<bool>> ValidateInspectionAsync(Guid agencyId, Guid propertyId, Guid inspectionId)
        {
            var inspection = await _unitOfWork.Inspections.FirstOrDefaultAsync(
                i => i.Id == inspectionId && i.AgencyId == agencyId && i.PropertyId == propertyId);

            if (inspection == null)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Record not found",
                    ErrorCode = ServiceErrorCodes.NotFound
                };
            }

            return new ServiceResponse<bool>
            {
                Success = true,
                Message = "Record retrieved successfully",
                Data = true
            };
        }

        private string GeneratePresignedUploadUrl(string key, string contentType)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _awsSettings.Value.S3Bucket,
                Key = key,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddMinutes(10),
                ContentType = contentType
            };

            return _s3Client.GetPreSignedURL(request);
        }

        private string BuildFileUrl(string key)
        {
            return $"https://{_awsSettings.Value.S3Bucket}.s3.{_awsSettings.Value.Region}.amazonaws.com/{key}";
        }

        private static string BuildFolderPath(Guid agencyId, Guid propertyId, Guid inspectionId, string mediaFolder)
        {
            return $"agencies/{agencyId}/{propertyId}/{inspectionId}/{mediaFolder}";
        }

        private static string GetMediaFolder(MediaType mediaType)
        {
            return mediaType == MediaType.Video ? "videos" : "photos";
        }

        private static string NormalizeMediaType(string mediaType)
        {
            if (string.IsNullOrWhiteSpace(mediaType))
            {
                return string.Empty;
            }

            var normalized = mediaType.Trim().ToLowerInvariant();
            if (normalized is "photo" or "photos")
            {
                return "photos";
            }

            if (normalized is "video" or "videos")
            {
                return "videos";
            }

            return string.Empty;
        }

        private static string GetFileExtension(string contentType)
        {
            return contentType.ToLowerInvariant() switch
            {
                "image/jpeg" => ".jpg",
                "image/jpg" => ".jpg",
                "image/png" => ".png",
                "image/webp" => ".webp",
                "video/mp4" => ".mp4",
                "video/quicktime" => ".mov",
                _ => string.Empty
            };
        }
    }
}






