using PropertyInspection.Shared.DTOs;
using PropertyInspection.Shared;

namespace PropertyInspection.Application.IServices
{
    public interface IAgencyService
    {
        Task<ServiceResponse<PagedResult<AgencyResponse>>> GetAllAsync(
            int pageNumber,
            int pageSize,
            Guid? countryId,
            Guid? stateId,
            string? name,
            string? suburb);

        Task<ServiceResponse<AgencyResponse>> GetByIdAsync(Guid agencyId);
        Task<ServiceResponse<AgencyResponse>> CreateAsync(CreateAgencyRequest dto);
        Task<ServiceResponse<AgencyResponse>> UpdateAsync(Guid agencyId, UpdateAgencyRequest dto);
        Task<ServiceResponse<bool>> DeleteAsync(Guid agencyId);
    }
}

