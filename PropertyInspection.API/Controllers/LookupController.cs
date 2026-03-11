using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyInspection.Application.IServices;
using PropertyInspection.Infrastructure.Data;
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
        public async Task<ActionResult<ApiResponse<IEnumerable<LookupDto>>>> GetRentFrequencies(Guid? agencyId)
        {
            var result = await _lookupService.GetRentFrequenciesAsync();

            return Ok(new ApiResponse<IEnumerable<LookupDto>>
            {
                Success = true,
                Message = "Rent frequencies retrieved successfully",
                Data = result
            });
        }

        [HttpGet("propertytypes")]
        public async Task<ActionResult<ApiResponse<IEnumerable<LookupDto>>>> GetPropertyTypes(Guid? agencyId)
        {
            var result = await _lookupService.GetPropertyTypesAsync();

            return Ok(new ApiResponse<IEnumerable<LookupDto>>
            {
                Success = true,
                Message = "Property types retrieved successfully",
                Data = result
            });
        }

        [HttpGet("inspection/statuses")]
        public async Task<ActionResult<ApiResponse<IEnumerable<LookupDto>>>> GetInspectionStatuses(Guid? agencyId)
        {
            var result = await _lookupService.GetInspectionStatusesAsync();

            return Ok(new ApiResponse<IEnumerable<LookupDto>>
            {
                Success = true,
                Message = "Inspection statuses retrieved successfully",
                Data = result
            });
        }

        [HttpGet("inspection/types")]
        public async Task<ActionResult<ApiResponse<IEnumerable<LookupDto>>>> GetInspectionTypes(Guid? agencyId)
        {
            var result = await _lookupService.GetInspectionTypesAsync();

            return Ok(new ApiResponse<IEnumerable<LookupDto>>
            {
                Success = true,
                Message = "Inspection types retrieved successfully",
                Data = result
            });
        }

        // ---------------- DB based lookups ----------------

        [HttpGet("states")]
        public async Task<ActionResult<ApiResponse<IEnumerable<StateLookupDto>>>> GetStates(Guid? agencyId)
        {
            var result = await _lookupService.GetStatesAsync();

            return Ok(new ApiResponse<IEnumerable<StateLookupDto>>
            {
                Success = true,
                Message = "States retrieved successfully",
                Data = result
            });
        }

        [HttpGet("countries")]
        public async Task<ActionResult<ApiResponse<IEnumerable<CountryDto>>>> GetCountries(Guid? agencyId)
        {
            var result = await _lookupService.GetCountriesAsync();

            return Ok(new ApiResponse<IEnumerable<CountryDto>>
            {
                Success = true,
                Message = "Countries retrieved successfully",
                Data = result
            });
        }

        [HttpGet("countries/{countryId}/states")]
        public async Task<ActionResult<ApiResponse<IEnumerable<StateLookupDto>>>> GetStatesByCountry(Guid countryId, Guid? agencyId)
        {
            var result = await _lookupService.GetStatesByCountryAsync(countryId);

            return Ok(new ApiResponse<IEnumerable<StateLookupDto>>
            {
                Success = true,
                Message = $"States for country ID {countryId} retrieved successfully",
                Data = result
            });
        }

        [HttpGet("countries/{countryId}/timezones")]
        public async Task<ActionResult<ApiResponse<IEnumerable<TimeZoneLookupDto>>>> GetTimezonesByCountry(Guid countryId, Guid? agencyId)
        {
            var result = await _lookupService.GetTimezonesByCountryAsync(countryId);

            return Ok(new ApiResponse<IEnumerable<TimeZoneLookupDto>>
            {
                Success = true,
                Message = $"Timezones for country ID {countryId} retrieved successfully",
                Data = result
            });
        }

        
    }

}

