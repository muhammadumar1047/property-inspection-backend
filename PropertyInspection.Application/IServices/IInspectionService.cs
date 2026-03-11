using PropertyInspection.Core.Enums;
using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.Application.IServices
{
    public interface IInspectionService
    {
        Task<(IEnumerable<InspectionDto> Inspections, int TotalCount)> GetAllInspectionsAsync(
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
        Task<InspectionDto?> GetByIdAsync(Guid inspectionId , Guid? agencyId);

        // Create new inspection
        Task<InspectionDto> CreateAsync(CreateInspectionDto inspectionDto);

        // Update existing inspection
        Task<InspectionDto?> UpdateAsync(InspectionDto inspectionDto);

        // Get all inspections for a property
        Task<List<InspectionDto>> GetAllByPropertyAsync(Guid propertyId, Guid? agencyId);

        // Delete inspection by Id
        Task<bool> DeleteAsync(Guid inspectionId, Guid? agencyId);

        // Delete landlord snapshot
        Task<bool> DeleteLandlordSnapshotAsync(Guid landlordSnapshotId, Guid? agencyId);

        // Delete tenancy snapshot
        Task<bool> DeleteTenancySnapshotAsync(Guid tenancySnapshotId, Guid? agencyId);
    }
}
