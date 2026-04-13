using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PropertyInspection.API.Authorization;
using PropertyInspection.API.Extensions;
using PropertyInspection.Application.IServices;
using PropertyInspection.Core.Enums;
using PropertyInspection.Core.Interfaces.UnitOfWork;
using PropertyInspection.Shared;
using PropertyInspection.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace PropertyInspection.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PropertyController : ControllerBase
    {
        private readonly IPropertyService _propertyService;
        private readonly IS3Service _s3Service;
        private readonly IOptions<AwsSettings> _awsSettings;
        private readonly ITenantAgencyResolver _tenantAgencyResolver;
        private readonly IUnitOfWork _unitOfWork;

        public PropertyController(
            IPropertyService propertyService,
            IS3Service s3Service,
            IOptions<AwsSettings> awsSettings,
            ITenantAgencyResolver tenantAgencyResolver,
            IUnitOfWork unitOfWork)
        {
            _propertyService = propertyService;
            _s3Service = s3Service;
            _awsSettings = awsSettings;
            _tenantAgencyResolver = tenantAgencyResolver;
            _unitOfWork = unitOfWork;
        }

        //[Permission("property.view")]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<PropertyResponse>>>> GetAllByAgency(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] PropertyType? propertyType = null,
            [FromQuery] Guid? propertyManagerId = null,
            [FromQuery] string? tenant = null,
            [FromQuery] string? owner = null,
            [FromQuery] string? suburb = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] Guid? agencyId = null)
        {
          

            var result = await _propertyService.GetAllByAgencyAsync(
                agencyId,
                pageNumber,
                pageSize,
                propertyType,
                propertyManagerId,
                tenant,
                owner,
                suburb,
                isActive
            );
            return this.ToActionResult(result, new { Count = result.Data?.Data.Count ?? 0 });
        }

        //[Permission("property.view")]
        [HttpGet("{propertyId}")]
        public async Task<ActionResult<ApiResponse<PropertyResponse>>> GetById(Guid propertyId, Guid? agencyId)
        {
            var result = await _propertyService.GetByIdAsync(propertyId, agencyId);
            return this.ToActionResult(result);
        }

        //[Permission("property.create")]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<PropertyResponse>>> Create([FromBody] CreatePropertyRequest propertyRequest)
        {
            var result = await _propertyService.CreateAsync(propertyRequest);
            return this.ToCreatedAtActionResult(
                nameof(GetById),
                new { propertyId = result.Data?.Id ?? Guid.Empty },
                result);
        }

        //[Permission("property.update")]
        [HttpPut("{propertyId}")]
        public async Task<ActionResult<ApiResponse<bool>>> Update(Guid propertyId, [FromBody] UpdatePropertyRequest propertyRequest)
        {
            var result = await _propertyService.UpdateAsync(propertyId, propertyRequest);
            return this.ToActionResult(result);
        }

        //[Permission("property.delete")]
        [HttpDelete("{propertyId}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid propertyId, Guid? agencyId)
        {
            var result = await _propertyService.DeletePropertyAsync(propertyId, agencyId);
            return this.ToActionResult(result);
        }

        //[Permission("property.update")]
        [HttpPost("{propertyId}/images")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<PropertyImageUploadResponse>>>> UploadPropertyImages(
            Guid propertyId,
            [FromQuery] Guid? agencyId,
            [FromForm(Name = "agencyId")] Guid? agencyIdFromForm,
            [FromForm] List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                return this.ToActionResult(new ServiceResponse<IReadOnlyList<PropertyImageUploadResponse>>
                {
                    Success = false,
                    Message = "No files provided",
                    ErrorCode = ServiceErrorCodes.InvalidRequest
                });
            }

            if (files.Count > 20)
            {
                return this.ToActionResult(new ServiceResponse<IReadOnlyList<PropertyImageUploadResponse>>
                {
                    Success = false,
                    Message = "Too many files in a single request",
                    ErrorCode = ServiceErrorCodes.InvalidRequest
                });
            }

            var resolvedAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId ?? agencyIdFromForm);
            var property = await _unitOfWork.Properties.FirstOrDefaultAsync(
                p => p.Id == propertyId && p.AgencyId == resolvedAgencyId);

            if (property == null)
            {
                return this.ToActionResult(new ServiceResponse<IReadOnlyList<PropertyImageUploadResponse>>
                {
                    Success = false,
                    Message = "Record not found",
                    ErrorCode = ServiceErrorCodes.NotFound
                });
            }

            var folderPath = $"agencies/{resolvedAgencyId}/properties/{propertyId}/propertyImages";
            var existingImages = ParsePropertyImages(property.PropertyImages);
            var responses = new List<PropertyImageUploadResponse>();

            foreach (var file in files)
            {
                if (file.Length <= 0)
                {
                    continue;
                }

                var contentType = file.ContentType ?? string.Empty;
                if (!IsSupportedImageContentType(contentType))
                {
                    return this.ToActionResult(new ServiceResponse<IReadOnlyList<PropertyImageUploadResponse>>
                    {
                        Success = false,
                        Message = "Only image files are allowed",
                        ErrorCode = ServiceErrorCodes.InvalidRequest
                    });
                }

                var imageGuid = Guid.NewGuid().ToString("N");
                var extension = GetImageExtension(contentType, file.FileName);
                var fileName = string.IsNullOrWhiteSpace(extension) ? imageGuid : $"{imageGuid}{extension}";

                await using var stream = file.OpenReadStream();
                var uploadResult = await _s3Service.UploadFileAsync(stream, folderPath, fileName, contentType);
                if (!uploadResult.Success)
                {
                    return this.ToActionResult(new ServiceResponse<IReadOnlyList<PropertyImageUploadResponse>>
                    {
                        Success = false,
                        Message = uploadResult.Message,
                        ErrorCode = uploadResult.ErrorCode
                    });
                }

                var key = $"{folderPath}/{fileName}";
                var url = BuildFileUrl(key);
                responses.Add(new PropertyImageUploadResponse
                {
                    ImageGuid = imageGuid,
                    FileKey = key,
                    FileUrl = url,
                    ContentType = contentType
                });
                existingImages.Add(url);
            }

            if (responses.Count > 0)
            {
                property.PropertyImages = SerializePropertyImages(existingImages);
                property.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Properties.UpdateAsync(property);
                await _unitOfWork.CommitAsync();
            }

            return Ok(new ApiResponse<IReadOnlyList<PropertyImageUploadResponse>>
            {
                Success = true,
                Message = "Images uploaded successfully",
                Data = responses
            });
        }

        private string BuildFileUrl(string key)
        {
            return $"https://{_awsSettings.Value.S3Bucket}.s3.{_awsSettings.Value.Region}.amazonaws.com/{key}";
        }

        private static bool IsSupportedImageContentType(string contentType)
        {
            if (string.IsNullOrWhiteSpace(contentType))
            {
                return false;
            }

            return contentType.Equals("image/jpeg", StringComparison.OrdinalIgnoreCase)
                || contentType.Equals("image/jpg", StringComparison.OrdinalIgnoreCase)
                || contentType.Equals("image/png", StringComparison.OrdinalIgnoreCase)
                || contentType.Equals("image/webp", StringComparison.OrdinalIgnoreCase);
        }

        private static string GetImageExtension(string contentType, string fileName)
        {
            return contentType.ToLowerInvariant() switch
            {
                "image/jpeg" => ".jpg",
                "image/jpg" => ".jpg",
                "image/png" => ".png",
                "image/webp" => ".webp",
                _ => Path.GetExtension(fileName) ?? string.Empty
            };
        }

        private static List<string> ParsePropertyImages(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return new List<string>();
            }

            var trimmed = value.Trim();
            if (trimmed.StartsWith("["))
            {
                try
                {
                    var list = JsonSerializer.Deserialize<List<string>>(trimmed) ?? new List<string>();
                    return list.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                }
                catch
                {
                    // Fallback to treating it as a single URL below.
                }
            }

            var split = trimmed.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            return split.Length > 1
                ? split.Where(x => !string.IsNullOrWhiteSpace(x)).ToList()
                : new List<string> { trimmed };
        }

        private static string SerializePropertyImages(IEnumerable<string> images)
        {
            var cleaned = images
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct()
                .ToList();

            return JsonSerializer.Serialize(cleaned);
        }
    }
}

