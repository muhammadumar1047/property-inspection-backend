using Microsoft.EntityFrameworkCore;
using PropertyInspection.Application.IServices;
using PropertyInspection.Core.Entities;
using PropertyInspection.Core.Enums;
using PropertyInspection.Core.Interfaces.UnitOfWork;
using PropertyInspection.Shared.DTOs;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Application.Services
{
    public class LookupService : ILookupService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LookupService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        private IEnumerable<LookupDto> EnumToLookup<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T))
                       .Cast<T>()
                       .Select(e => new LookupDto
                       {
                           Id = Convert.ToInt32(e),
                           Name = e.ToString()
                       });
        }

        public Task<IEnumerable<LookupDto>> GetRentFrequenciesAsync() =>
           Task.FromResult(EnumToLookup<RentFrequency>());

        public Task<IEnumerable<LookupDto>> GetPropertyTypesAsync() =>
            Task.FromResult(EnumToLookup<PropertyType>());

        public Task<IEnumerable<LookupDto>> GetInspectionStatusesAsync() =>
            Task.FromResult(EnumToLookup<InspectionStatus>());

        public Task<IEnumerable<LookupDto>> GetInspectionTypesAsync() =>
            Task.FromResult(EnumToLookup<InspectionType>());


        public async Task<IEnumerable<StateLookupDto>> GetStatesAsync()
        {
            var states = await _unitOfWork.States.GetAsync(
                include: q => q.Include(s => s.Country).AsNoTracking(),
                orderBy: q => q.OrderBy(s => s.Name)
            );

            return _mapper.Map<IEnumerable<StateLookupDto>>(states);
        }

        public async Task<IEnumerable<CountryDto>> GetCountriesAsync()
        {
            var countries = await _unitOfWork.Countries
                .GetAsync(orderBy: q => q.OrderBy(c => c.Name));

            return _mapper.Map<IEnumerable<CountryDto>>(countries);
        }

        public async Task<IEnumerable<TimeZoneLookupDto>> GetTimezonesByCountryAsync(Guid countryId)
        {
            var timezones = await _unitOfWork.TimeZones
                .GetAsync(predicate: tz => tz.CountryId == countryId,
                          include : q => q.Include(tz => tz.Country).AsNoTracking(),
                          orderBy: q => q.OrderBy(t => t.DisplayName));

            return _mapper.Map<IEnumerable<TimeZoneLookupDto>>(timezones);
        }

       
        public async Task<IEnumerable<StateLookupDto>> GetStatesByCountryAsync(Guid countryId)
        {
            var states = await _unitOfWork.States.GetAsync(
                predicate: s => s.CountryId == countryId,
                include: q => q.Include(s => s.Country).AsNoTracking(),
                orderBy: q => q.OrderBy(s => s.Name)
            );

            return _mapper.Map<IEnumerable<StateLookupDto>>(states);
        }


    }
}
