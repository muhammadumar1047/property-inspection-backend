using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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
            if (string.IsNullOrEmpty(agencyId) || string.IsNullOrEmpty(propertyId) || string.IsNullOrEmpty(inspectionId))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "agencyId, propertyId, and inspectionId are required.",
                    Data = false
                });
            }

            var photoId = Guid.NewGuid().ToString();

            var folderPath = $"agencies/{agencyId}/{propertyId}/{inspectionId}/{mediaType}";
            var fileName = $"{photoId}.jpg";

            var uploadUrl = await _s3Service.GeneratePresignedUploadUrlAsync(folderPath, fileName, contentType);
            var fileUrl = $"https://{_awsSettings.Value.S3Bucket}.s3.{_awsSettings.Value.Region}.amazonaws.com/{folderPath}/{fileName}";

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Record retrieved successfully",
                Data = new
                {
                    uploadUrl,
                    fileUrl,
                    photoId
                }
            });
        }

        [HttpGet("generate-download-url")]
        public async Task<ActionResult<ApiResponse<object>>> GenerateDownloadUrl([FromQuery] string folder, [FromQuery] string fileName)
        {
            if (string.IsNullOrEmpty(folder) || string.IsNullOrEmpty(fileName))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Folder and fileName are required.",
                    Data = false
                });
            }

            var url = await _s3Service.GeneratePresignedDownloadUrlAsync(folder, fileName);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Record retrieved successfully",
                Data = new { downloadUrl = url }
            });
        }

        [HttpDelete("delete-file")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteFile([FromQuery] string folder, [FromQuery] string fileName)
        {
            if (string.IsNullOrEmpty(folder) || string.IsNullOrEmpty(fileName))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Folder and fileName are required.",
                    Data = false
                });
            }

            await _s3Service.DeleteFileAsync(folder, fileName);
            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Record deleted successfully",
                Data = true
            });
        }

        [HttpGet("file-exists")]
        public async Task<ActionResult<ApiResponse<bool>>> FileExists([FromQuery] string folder, [FromQuery] string fileName)
        {
            var exists = await _s3Service.FileExistsAsync(folder, fileName);
            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Record retrieved successfully",
                Data = exists
            });
        }
    }
}
