

using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.Application.IServices
{
    public interface ISearchService
    {
        Task<SearchResultGroupedDto> SearchAsync(string query, Guid? agencyId);
        Task<List<SearchPropertyDto>> SearchPropertyAsync(string query, Guid? agencyId);
    }
}
