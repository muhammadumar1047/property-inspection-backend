

using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.Application.IServices
{
    public interface IPropertyLayoutService
    {
        /// <summary>
        /// Get all layouts for a specific agency.
        /// </summary>
        Task<(IEnumerable<PropertyLayoutDto>,int TotalCount)> GetAllByAgencyAsync(
            Guid? agencyId,
            int pageNumber = 1,
            int pageSize = 10
        );

        /// <summary>
        /// Get a single layout by its ID and agency.
        /// </summary>
        Task<PropertyLayoutDto?> GetByIdAsync(Guid layoutId, Guid? agencyId);

        /// <summary>
        /// Create a new layout (with nested areas and items).
        /// </summary>
        Task<PropertyLayoutDto> CreateAsync(CreatePropertyLayoutDto layoutDto);

        /// <summary>
        /// Update an existing layout (with nested areas and items).
        /// </summary>
        Task<PropertyLayoutDto> UpdateAsync(PropertyLayoutDto layoutDto);

        /// <summary>
        /// Delete a layout by ID and agency.
        /// </summary>
        Task<bool> DeleteAsync(Guid layoutId, Guid? agencyId);
    }
}
