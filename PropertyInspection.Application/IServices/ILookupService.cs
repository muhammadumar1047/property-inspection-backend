using PropertyInspection.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Application.IServices
{
    public interface ILookupService
    {
        Task<IEnumerable<LookupDto>> GetRentFrequenciesAsync();
        Task<IEnumerable<LookupDto>> GetPropertyTypesAsync();
        Task<IEnumerable<LookupDto>> GetInspectionStatusesAsync();
        Task<IEnumerable<LookupDto>> GetInspectionTypesAsync();

        Task<IEnumerable<StateLookupDto>> GetStatesAsync();
        Task<IEnumerable<CountryDto>> GetCountriesAsync();
        Task<IEnumerable<TimeZoneLookupDto>> GetTimezonesByCountryAsync(Guid countryId);
        Task<IEnumerable<StateLookupDto>> GetStatesByCountryAsync(Guid countryId);
    }
}
