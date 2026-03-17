using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PropertyInspection.API.Authorization;
using PropertyInspection.API.Extensions;
using PropertyInspection.Application.IServices;
using PropertyInspection.Core.Enums;
using PropertyInspection.Shared;
using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InspectionController : ControllerBase
    {
        private readonly IInspectionService _inspectionService;

        public InspectionController(IInspectionService inspectionService)
        {
            _inspectionService = inspectionService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<InspectionResponse>>>> GetAllInspections(
            Guid? agencyId,
            Guid inspectionId = default,
            int pageNumber = 1,
            int pageSize = 10,
            InspectionType? inspectionType = null,
            InspectionStatus? inspectionStatus = null,
            Guid? inspectorId = null,
            string? suburb = null,
            DateTime? inspectionDate = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string? searchProperty = null
        )
        {
            var result = await _inspectionService.GetAllInspectionsAsync(
                agencyId,
                inspectionId, pageNumber, pageSize,
                inspectionType, inspectionStatus, inspectorId,
                suburb, inspectionDate, startDate, endDate, searchProperty
            );
            return this.ToActionResult(result, new { Count = result.Data?.Data.Count ?? 0 });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<InspectionResponse>>> GetById(Guid id, Guid? agencyId)
        {
            var result = await _inspectionService.GetByIdAsync(id , agencyId);
            return this.ToActionResult(result);
        }

        [HttpGet("property/{propertyId}")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<InspectionResponse>>>> GetByProperty(Guid propertyId , Guid? agencyId)
        {
            var result = await _inspectionService.GetAllByPropertyAsync(propertyId ,agencyId);
            return this.ToActionResult(result, new { Count = result.Data?.Count ?? 0 });
        }

        [HttpPost]
        //[Permission("inspection.create")]
        public async Task<ActionResult<ApiResponse<InspectionResponse>>> Create([FromBody] CreateInspectionRequest inspectionDto)
        {
            var result = await _inspectionService.CreateAsync(inspectionDto);
            return this.ToCreatedAtActionResult(
                nameof(GetById),
                new { id = result.Data?.Id ?? Guid.Empty },
                result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Update(Guid id, [FromBody] UpdateInspectionRequest inspectionRequest)
        {
            var result = await _inspectionService.UpdateAsync(id, inspectionRequest);
            return this.ToActionResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id , Guid? agencyId)
        {
            var result = await _inspectionService.DeleteAsync(id , agencyId);
            return this.ToActionResult(result);
        }

        [HttpDelete("landlord-snapshot/{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteLandlordSnapshot(Guid id, Guid? agencyId)
        {
            var result = await _inspectionService.DeleteLandlordSnapshotAsync(id, agencyId);
            return this.ToActionResult(result);
        }

        [HttpDelete("tenancy-snapshot/{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteTenancySnapshot(Guid id, Guid? agencyId)
        {
            var result = await _inspectionService.DeleteTenancySnapshotAsync(id, agencyId);
            return this.ToActionResult(result);
        }
    }
}

