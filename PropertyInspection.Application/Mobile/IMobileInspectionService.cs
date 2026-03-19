using PropertyInspection.Shared;
using PropertyInspection.Shared.DTOs;
using PropertyInspection.Core.Enums;

namespace PropertyInspection.Application.IServices
{
    public interface IMobileInspectionService
    {
        Task<ServiceResponse<PagedResult<InspectionResponse>>> GetMobileInspectionsAsync(
            Guid? agencyId,
            int pageNumber,
            int pageSize,
            Guid? propertyId,
            InspectionType? inspectionType,
            InspectionStatus? inspectionStatus,
            DateTime? startDate,
            DateTime? endDate,
            string? propertySearch);
    }
}
