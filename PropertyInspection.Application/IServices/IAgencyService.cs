using PropertyInspection.Core.Entities;
using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.Application.IServices
{
    public interface IAgencyService
    {
        Task<(List<AgencyDto> Agencies, int TotalCount)> GetAllAsync(
            int pageNumber,
            int pageSize,
            Guid? countryId,
            Guid? stateId,
            string? name,
            string? suburb);

        Task<AgencyDto?> GetByIdAsync(Guid agencyId);
        Task<AgencyDto> CreateAsync(CreateAgencyDto dto);
        Task<AgencyDto?> UpdateAsync(Guid agencyId, UpdateAgencyDto dto);
        Task<bool> DeleteAsync(Guid agencyId);
    }
}
