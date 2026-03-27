using Microsoft.AspNetCore.Mvc;
using PropertyInspection.API.Extensions;
using PropertyInspection.Application.IServices;
using PropertyInspection.Shared;
using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillingController : ControllerBase
    {
        private readonly IBillingService _billingService;

        public BillingController(IBillingService billingService)
        {
            _billingService = billingService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<BillingDto>>>> GetBillings([FromQuery] BillingFilterDto filter)
        {
            var result = await _billingService.GetBillingsAsync(filter);
            return this.ToActionResult(result, new { Count = result.Data?.Data.Count ?? 0 });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<BillingDto>>> GetBilling(Guid id)
        {
            var result = await _billingService.GetBillingByIdAsync(id);
            return this.ToActionResult(result);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<BillingDto>>> CreateBilling([FromBody] CreateBillingDto dto)
        {
            if (!ModelState.IsValid)
            {
                return this.ToActionResult(new ServiceResponse<BillingDto>
                {
                    Success = false,
                    Message = "Invalid request data",
                    ErrorCode = ServiceErrorCodes.InvalidRequest
                });
            }

            var result = await _billingService.CreateBillingAsync(dto);
            return this.ToCreatedAtActionResult(
                nameof(GetBilling),
                new { id = result.Data?.Id ?? Guid.Empty },
                result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<BillingDto>>> UpdateBilling(Guid id, [FromBody] UpdateBillingDto dto)
        {
            if (!ModelState.IsValid)
            {
                return this.ToActionResult(new ServiceResponse<BillingDto>
                {
                    Success = false,
                    Message = "Invalid request data",
                    ErrorCode = ServiceErrorCodes.InvalidRequest
                });
            }

            var result = await _billingService.UpdateBillingAsync(id, dto);
            return this.ToActionResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteBilling(Guid id)
        {
            var result = await _billingService.DeleteBillingAsync(id);
            return this.ToActionResult(result, new { BillingId = id });
        }

        [HttpPatch("{id}/activate")]
        public async Task<ActionResult<ApiResponse<bool>>> ActivateBilling(Guid id)
        {
            var result = await _billingService.ActivateBillingAsync(id);
            return this.ToActionResult(result);
        }

        [HttpPatch("{id}/deactivate")]
        public async Task<ActionResult<ApiResponse<bool>>> DeactivateBilling(Guid id)
        {
            var result = await _billingService.DeactivateBillingAsync(id);
            return this.ToActionResult(result);
        }
    }
}
