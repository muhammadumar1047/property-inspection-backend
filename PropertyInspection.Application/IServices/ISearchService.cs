

using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.Application.IServices
{
    public interface ISearchService
    {
        Task<PropertyInspection.Shared.ServiceResponse<SearchResultGroupedDto>> SearchAsync(string query, Guid? agencyId);
        Task<PropertyInspection.Shared.ServiceResponse<IReadOnlyList<SearchPropertyDto>>> SearchPropertyAsync(string query, Guid? agencyId);
    }
}
