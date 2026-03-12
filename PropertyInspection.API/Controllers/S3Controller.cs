using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PropertyInspection.API.Extensions;
using PropertyInspection.Application.IServices;
using PropertyInspection.Shared;

namespace PropertyInspection.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class S3Controller : ControllerBase
    {
        private readonly IS3Service _s3Service;
        private readonly IOptions<AwsSettings> _awsSettings;

        public S3Controller(IS3Service s3Service, IOptions<AwsSettings> awsSettings)
        {
            _s3Service = s3Service;
            _awsSettings = awsSettings;
        }

        [HttpGet("generate-upload-url")]
        public async Task<ActionResult<ApiResponse<object>>> GenerateUploadUrl(
            [FromQuery] string agencyId,
            [FromQuery] string propertyId,
            [FromQuery] string inspectionId,
            [FromQuery] string mediaType,
            [FromQuery] string contentType = "image/jpeg"
            )
        {
            if (string.IsNullOrWhiteSpace(agencyId) || string.IsNullOrWhiteSpace(propertyId) || string.IsNullOrWhiteSpace(inspectionId))
            {
                return this.ToActionResult(new ServiceResponse<object>
                {
                    Success = false,
                    Message = "Invalid request data",
                    ErrorCode = ServiceErrorCodes.InvalidRequest
                });
            }

            var photoId = Guid.NewGuid().ToString();

            var folderPath = $"agencies/{agencyId}/{propertyId}/{inspectionId}/{mediaType}";
            var fileName = $"{photoId}.jpg";

            var uploadResult = await _s3Service.GeneratePresignedUploadUrlAsync(folderPath, fileName, contentType);
            if (!uploadResult.Success)
            {
                return this.ToActionResult(new ServiceResponse<object>
                {
                    Success = false,
                    Message = uploadResult.Message,
                    ErrorCode = uploadResult.ErrorCode
                });
            }

            var fileUrl = $"https://{_awsSettings.Value.S3Bucket}.s3.{_awsSettings.Value.Region}.amazonaws.com/{folderPath}/{fileName}";

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = uploadResult.Message,
                Data = new
                {
                    uploadUrl = uploadResult.Data,
                    fileUrl,
                    photoId
                }
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
    }
}
