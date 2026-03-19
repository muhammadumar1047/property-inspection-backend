using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyInspection.API.Extensions;
using PropertyInspection.Application.IServices;
using PropertyInspection.Core.Enums;
using PropertyInspection.Shared;
using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.API.Controllers
{
    [Route("api/mobile/inspections")]
    [ApiController]
    [Authorize]
    public class MobileInspectionController : ControllerBase
    {
        private readonly IMobileInspectionService _inspectionService;

        public MobileInspectionController(IMobileInspectionService inspectionService)
        {
            _inspectionService = inspectionService;
        }

        [HttpGet("list")]
        public Task<ActionResult<ApiResponse<PagedResult<InspectionResponse>>>> GetMobileInspectionList(
            [FromQuery] Guid? agencyId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] Guid? propertyId = null,
            [FromQuery] InspectionType? inspectionType = null,
            [FromQuery] InspectionStatus? inspectionStatus = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] DateTime? inspectionDate = null,
            [FromQuery] string? propertySearch = null,
            [FromQuery] string? search = null)
        {
            return GetMobileInspectionsInternal(
                agencyId,
                pageNumber,
                pageSize,
                propertyId,
                inspectionType,
                inspectionStatus,
                startDate,
                endDate,
                propertySearch);
        }

        private async Task<ActionResult<ApiResponse<PagedResult<InspectionResponse>>>> GetMobileInspectionsInternal(
            Guid? agencyId,
            int pageNumber,
            int pageSize,
            Guid? propertyId,
            InspectionType? inspectionType,
            InspectionStatus? inspectionStatus,
            DateTime? startDate,
            DateTime? endDate,
            string? propertySearch)
        {
         
            var result = await _inspectionService.GetMobileInspectionsAsync(
                agencyId,
                pageNumber,
                pageSize,
                propertyId,
                inspectionType,
                inspectionStatus,
                startDate,
                endDate,
                propertySearch);

            return this.ToActionResult(result, new { Count = result.Data?.Data.Count ?? 0 });
        }
    }
}
