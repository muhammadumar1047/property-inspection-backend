

using PropertyInspection.Shared.DTOs;
using PropertyInspection.Shared;

namespace PropertyInspection.Application.IServices
{
    public interface IPropertyLayoutService
    {
        /// <summary>
        /// Get all layouts for a specific agency.
        /// </summary>
        Task<ServiceResponse<PagedResult<PropertyLayoutResponse>>> GetAllByAgencyAsync(
            Guid? agencyId,
            int pageNumber = 1,
            int pageSize = 10
        );

        /// <summary>
        /// Get a single layout by its ID and agency.
        /// </summary>
        Task<ServiceResponse<PropertyLayoutResponse>> GetByIdAsync(Guid layoutId, Guid? agencyId);

        /// <summary>
        /// Create a new layout (with nested areas and items).
        /// </summary>
        Task<ServiceResponse<PropertyLayoutResponse>> CreateAsync(CreatePropertyLayoutRequest layoutDto);

        /// <summary>
        /// Update an existing layout (with nested areas and items).
        /// </summary>
        Task<ServiceResponse<bool>> UpdateAsync(Guid layoutId, UpdatePropertyLayoutRequest layoutRequest);

        /// <summary>
        /// Delete a layout by ID and agency.
        /// </summary>
        Task<ServiceResponse<bool>> DeleteAsync(Guid layoutId, Guid? agencyId);
    }
}

