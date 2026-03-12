using PropertyInspection.Core.Enums;
using PropertyInspection.Shared.DTOs;
using PropertyInspection.Shared;

namespace PropertyInspection.Application.IServices
{
    public interface IPropertyService
    {
        Task<ServiceResponse<PagedResult<PropertyResponse>>> GetAllByAgencyAsync(
            Guid? agencyId,
            int pageNumber = 1,
            int pageSize = 10,
            PropertyType? propertyTypeId = null,
            Guid? propertyManagerId = null,
            string? tenant = null,
            string? owner = null,
            string? suburb = null,
            bool? isActive = null);

        Task<ServiceResponse<PropertyResponse>> GetByIdAsync(Guid propertyId, Guid? agencyId);
        Task<ServiceResponse<PropertyResponse>> CreateAsync(CreatePropertyRequest propertyRequest);
        Task<ServiceResponse<bool>> UpdateAsync(Guid propertyId, UpdatePropertyRequest propertyRequest);
        Task<ServiceResponse<bool>> DeletePropertyAsync(Guid propertyId, Guid? agencyId);
    }
}

