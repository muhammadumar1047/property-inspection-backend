using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyInspection.API.Extensions;
using PropertyInspection.Application.IServices;
using PropertyInspection.Shared;
using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SignatureController : ControllerBase
    {
        private readonly ISignatureService _signatureService;

        public SignatureController(ISignatureService signatureService)
        {
            _signatureService = signatureService;
        }

        /// <summary>
        /// Generates a pre-signed URL for the inspector to upload a signature image directly to S3.
        /// </summary>
        [HttpPost("upload-url")]
        public async Task<ActionResult<ApiResponse<SignatureUploadUrlResponse>>> GenerateUploadUrl(
            [FromBody] GenerateSignatureUploadUrlRequest request)
        {
            if (!Guid.TryParse(User.GetDomainUserId(), out var userId))
            {
                return this.ToActionResult(new ServiceResponse<SignatureUploadUrlResponse>
                {
                    Success = false,
                    Message = "Unauthorized",
                    ErrorCode = ServiceErrorCodes.Unauthorized
                });
            }

            var result = await _signatureService.GenerateUploadUrlAsync(userId, request);
            return this.ToActionResult(result);
        }

        /// <summary>
        /// Saves the uploaded signature image URL in the user table after the inspector has uploaded
        /// the file using the pre-signed URL.
        /// </summary>
        [HttpPost("save")]
        public async Task<ActionResult<ApiResponse<bool>>> SaveSignature(
            [FromBody] SaveSignatureRequest request)
        {
            if (!Guid.TryParse(User.GetDomainUserId(), out var userId))
            {
                return this.ToActionResult(new ServiceResponse<bool>
                {
                    Success = false,
                    Message = "Unauthorized",
                    ErrorCode = ServiceErrorCodes.Unauthorized
                });
            }

            var result = await _signatureService.SaveSignatureAsync(userId, request);
            return this.ToActionResult(result);
        }
    }
}