using PropertyInspection.Core.Enums;
using PropertyInspection.Shared.DTOs;
using PropertyInspection.Shared;

namespace PropertyInspection.Application.IServices
{
    public interface IInspectionService
    {
        Task<ServiceResponse<PagedResult<InspectionResponse>>> GetAllInspectionsAsync(
            Guid? agencyId,
            Guid inspectionId,
            int pageNumber = 1,
            int pageSize = 10,
            InspectionType? inspectionType = null,
            InspectionStatus? inspectionStatus = null,
            Guid? inspectorId = null,
            string? suburb = null,
            DateTime? inspectionDate = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string? searchProperty = null);

        // Get single inspection by Id
        Task<ServiceResponse<InspectionResponse>> GetByIdAsync(Guid inspectionId , Guid? agencyId);

        // Create new inspection
        Task<ServiceResponse<InspectionResponse>> CreateAsync(CreateInspectionRequest inspectionDto);

        // Update existing inspection
        Task<ServiceResponse<bool>> UpdateAsync(Guid inspectionId, UpdateInspectionRequest inspectionRequest);

        // Get all inspections for a property
        Task<ServiceResponse<IReadOnlyList<InspectionResponse>>> GetAllByPropertyAsync(Guid propertyId, Guid? agencyId);

        // Delete inspection by Id
        Task<ServiceResponse<bool>> DeleteAsync(Guid inspectionId, Guid? agencyId);

        // Delete landlord snapshot
        Task<ServiceResponse<bool>> DeleteLandlordSnapshotAsync(Guid landlordSnapshotId, Guid? agencyId);

        // Delete tenancy snapshot
        Task<ServiceResponse<bool>> DeleteTenancySnapshotAsync(Guid tenancySnapshotId, Guid? agencyId);
    }
}

