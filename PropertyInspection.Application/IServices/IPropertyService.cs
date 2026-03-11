using PropertyInspection.Core.Enums;
using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.Application.IServices
{
    public interface IPropertyService
    {
        Task<(IEnumerable<PropertyDto> Properties, int TotalCount)> GetAllByAgencyAsync(
            Guid? agencyId,
            int pageNumber = 1,
            int pageSize = 10,
            PropertyType? propertyTypeId = null,
            Guid? propertyManagerId = null,
            string? tenant = null,
            string? owner = null,
            string? suburb = null,
            bool? isActive = null);

        Task<PropertyDto?> GetByIdAsync(Guid propertyId, Guid? agencyId);
        Task<PropertyDto> CreateAsync(PropertyDto propertyDto);
        Task<PropertyDto?> UpdateAsync(PropertyDto propertyDto);
        Task<bool> DeletePropertyAsync(Guid propertyId, Guid? agencyId);
    }
}
