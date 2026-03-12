using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyInspection.Application.IServices;
using PropertyInspection.Infrastructure.Data;
using PropertyInspection.API.Extensions;
using PropertyInspection.Shared;
using PropertyInspection.Shared.DTOs;

namespace PropertyInspection.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LookupController : ControllerBase
    {
        private readonly ILookupService _lookupService;

        public LookupController(ILookupService lookupService)
        {
            _lookupService = lookupService;
        }

        // ---------------- Enum based lookups ----------------

        [HttpGet("rentfrequency")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<LookupDto>>>> GetRentFrequencies(Guid? agencyId)
        {
            var result = await _lookupService.GetRentFrequenciesAsync();
            return this.ToActionResult(result);
        }

        [HttpGet("propertytypes")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<LookupDto>>>> GetPropertyTypes(Guid? agencyId)
        {
            var result = await _lookupService.GetPropertyTypesAsync();
            return this.ToActionResult(result);
        }

        [HttpGet("inspection/statuses")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<LookupDto>>>> GetInspectionStatuses(Guid? agencyId)
        {
            var result = await _lookupService.GetInspectionStatusesAsync();
            return this.ToActionResult(result);
        }

        [HttpGet("inspection/types")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<LookupDto>>>> GetInspectionTypes(Guid? agencyId)
        {
            var result = await _lookupService.GetInspectionTypesAsync();
            return this.ToActionResult(result);
        }

        // ---------------- DB based lookups ----------------

        [HttpGet("states")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<StateLookupDto>>>> GetStates(Guid? agencyId)
        {
            var result = await _lookupService.GetStatesAsync();
            return this.ToActionResult(result);
        }

        [HttpGet("countries")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<CountryDto>>>> GetCountries(Guid? agencyId)
        {
            var result = await _lookupService.GetCountriesAsync();
            return this.ToActionResult(result);
        }

        [HttpGet("countries/{countryId}/states")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<StateLookupDto>>>> GetStatesByCountry(Guid countryId, Guid? agencyId)
        {
            var result = await _lookupService.GetStatesByCountryAsync(countryId);
            return this.ToActionResult(result);
        }

        [HttpGet("countries/{countryId}/timezones")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<TimeZoneLookupDto>>>> GetTimezonesByCountry(Guid countryId, Guid? agencyId)
        {
            var result = await _lookupService.GetTimezonesByCountryAsync(countryId);
            return this.ToActionResult(result);
        }

        
    }

}

