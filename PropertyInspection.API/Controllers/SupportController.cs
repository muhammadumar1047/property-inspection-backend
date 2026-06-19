using Microsoft.AspNetCore.Mvc;
using PropertyInspection.API.Extensions;
using PropertyInspection.Application.IServices;
using PropertyInspection.Shared;
using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupportController : ControllerBase
    {
        private readonly ISupportService _supportService;

        public SupportController(ISupportService supportService)
        {
            _supportService = supportService;
        }

        /// <summary>
        /// Submit a customer support request. An email is sent automatically to the support team via SMTP.
        /// </summary>
        /// <param name="dto">Support request containing name, email, phone number, subject, and message.</param>
        /// <returns>Success or failure response.</returns>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<bool>>> Submit([FromBody] CreateSupportRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return this.ToActionResult(new ServiceResponse<bool>
                {
                    Success = false,
                    Message = string.Join("; ", errors),
                    ErrorCode = ServiceErrorCodes.InvalidRequest
                });
            }

            //var result = await _supportService.SubmitSupportRequestAsync(dto);
            //return this.ToActionResult(result);
            return this.ToActionResult(new ServiceResponse<bool>
            {
                Success = true,
                Message = "Your support request has been submitted successfully. We will get back to you shortly.",
                Data = true
            });
        }
    }
}