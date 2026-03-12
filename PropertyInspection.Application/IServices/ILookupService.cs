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
        Task<PropertyInspection.Shared.ServiceResponse<IReadOnlyList<LookupDto>>> GetRentFrequenciesAsync();
        Task<PropertyInspection.Shared.ServiceResponse<IReadOnlyList<LookupDto>>> GetPropertyTypesAsync();
        Task<PropertyInspection.Shared.ServiceResponse<IReadOnlyList<LookupDto>>> GetInspectionStatusesAsync();
        Task<PropertyInspection.Shared.ServiceResponse<IReadOnlyList<LookupDto>>> GetInspectionTypesAsync();

        Task<PropertyInspection.Shared.ServiceResponse<IReadOnlyList<StateLookupDto>>> GetStatesAsync();
        Task<PropertyInspection.Shared.ServiceResponse<IReadOnlyList<CountryDto>>> GetCountriesAsync();
        Task<PropertyInspection.Shared.ServiceResponse<IReadOnlyList<TimeZoneLookupDto>>> GetTimezonesByCountryAsync(Guid countryId);
        Task<PropertyInspection.Shared.ServiceResponse<IReadOnlyList<StateLookupDto>>> GetStatesByCountryAsync(Guid countryId);
    }
}
