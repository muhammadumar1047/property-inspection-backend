using Microsoft.EntityFrameworkCore;
using PropertyInspection.Application.IServices;
using PropertyInspection.Core.Entities;
using PropertyInspection.Core.Enums;
using PropertyInspection.Core.Interfaces.UnitOfWork;
using PropertyInspection.Shared.DTOs;
using PropertyInspection.Shared;
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

        public Task<ServiceResponse<IReadOnlyList<LookupDto>>> GetRentFrequenciesAsync()
        {
            try
            {
                var data = EnumToLookup<RentFrequency>().ToList();
                return Task.FromResult(new ServiceResponse<IReadOnlyList<LookupDto>>
                {
                    Success = true,
                    Message = "Records retrieved successfully",
                    Data = data
                });
            }
            catch
            {
                return Task.FromResult(new ServiceResponse<IReadOnlyList<LookupDto>>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                });
            }
        }

        public Task<ServiceResponse<IReadOnlyList<LookupDto>>> GetPropertyTypesAsync()
        {
            try
            {
                var data = EnumToLookup<PropertyType>().ToList();
                return Task.FromResult(new ServiceResponse<IReadOnlyList<LookupDto>>
                {
                    Success = true,
                    Message = "Records retrieved successfully",
                    Data = data
                });
            }
            catch
            {
                return Task.FromResult(new ServiceResponse<IReadOnlyList<LookupDto>>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                });
            }
        }

        public Task<ServiceResponse<IReadOnlyList<LookupDto>>> GetInspectionStatusesAsync()
        {
            try
            {
                var data = EnumToLookup<InspectionStatus>().ToList();
                return Task.FromResult(new ServiceResponse<IReadOnlyList<LookupDto>>
                {
                    Success = true,
                    Message = "Records retrieved successfully",
                    Data = data
                });
            }
            catch
            {
                return Task.FromResult(new ServiceResponse<IReadOnlyList<LookupDto>>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                });
            }
        }

        public Task<ServiceResponse<IReadOnlyList<LookupDto>>> GetInspectionTypesAsync()
        {
            try
            {
                var data = EnumToLookup<InspectionType>().ToList();
                return Task.FromResult(new ServiceResponse<IReadOnlyList<LookupDto>>
                {
                    Success = true,
                    Message = "Records retrieved successfully",
                    Data = data
                });
            }
            catch
            {
                return Task.FromResult(new ServiceResponse<IReadOnlyList<LookupDto>>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                });
            }
        }


        public async Task<ServiceResponse<IReadOnlyList<StateLookupDto>>> GetStatesAsync()
        {
            try
            {
                var states = await _unitOfWork.States.GetAsync(
                    include: q => q.Include(s => s.Country).AsNoTracking(),
                    orderBy: q => q.OrderBy(s => s.Name)
                );

                return new ServiceResponse<IReadOnlyList<StateLookupDto>>
                {
                    Success = true,
                    Message = "Records retrieved successfully",
                    Data = _mapper.Map<List<StateLookupDto>>(states)
                };
            }
            catch
            {
                return new ServiceResponse<IReadOnlyList<StateLookupDto>>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<IReadOnlyList<CountryDto>>> GetCountriesAsync()
        {
            try
            {
                var countries = await _unitOfWork.Countries
                    .GetAsync(orderBy: q => q.OrderBy(c => c.Name));

                return new ServiceResponse<IReadOnlyList<CountryDto>>
                {
                    Success = true,
                    Message = "Records retrieved successfully",
                    Data = _mapper.Map<List<CountryDto>>(countries)
                };
            }
            catch
            {
                return new ServiceResponse<IReadOnlyList<CountryDto>>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<IReadOnlyList<TimeZoneLookupDto>>> GetTimezonesByCountryAsync(Guid countryId)
        {
            try
            {
                var timezones = await _unitOfWork.TimeZones
                    .GetAsync(predicate: tz => tz.CountryId == countryId,
                              include: q => q.Include(tz => tz.Country).AsNoTracking(),
                              orderBy: q => q.OrderBy(t => t.DisplayName));

                return new ServiceResponse<IReadOnlyList<TimeZoneLookupDto>>
                {
                    Success = true,
                    Message = "Records retrieved successfully",
                    Data = _mapper.Map<List<TimeZoneLookupDto>>(timezones)
                };
            }
            catch
            {
                return new ServiceResponse<IReadOnlyList<TimeZoneLookupDto>>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<IReadOnlyList<StateLookupDto>>> GetStatesByCountryAsync(Guid countryId)
        {
            try
            {
                var states = await _unitOfWork.States.GetAsync(
                    predicate: s => s.CountryId == countryId,
                    include: q => q.Include(s => s.Country).AsNoTracking(),
                    orderBy: q => q.OrderBy(s => s.Name)
                );

                return new ServiceResponse<IReadOnlyList<StateLookupDto>>
                {
                    Success = true,
                    Message = "Records retrieved successfully",
                    Data = _mapper.Map<List<StateLookupDto>>(states)
                };
            }
            catch
            {
                return new ServiceResponse<IReadOnlyList<StateLookupDto>>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }


    }
}
