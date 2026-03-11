using PropertyInspection.Application.IServices;
using PropertyInspection.Core.Entities;
using Microsoft.EntityFrameworkCore;
using PropertyInspection.Core.Interfaces.UnitOfWork;
using PropertyInspection.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyInspection.Core.Enums;
using AutoMapper;

namespace PropertyInspection.Application.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPropertyLayoutService _propertyLayoutService;
        private readonly ITenantAgencyResolver _tenantAgencyResolver;
        private readonly IMapper _mapper;

        public PropertyService(
            IUnitOfWork unitOfWork,
            IPropertyLayoutService propertyLayoutService,
            ITenantAgencyResolver tenantAgencyResolver,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _propertyLayoutService = propertyLayoutService;
            _tenantAgencyResolver = tenantAgencyResolver;
            _mapper = mapper;
        }


        public async Task<(IEnumerable<PropertyDto> Properties, int TotalCount)> GetAllByAgencyAsync(
        Guid? agencyId,
        int pageNumber = 1,
        int pageSize = 10,
        PropertyType? propertyType = null,
        Guid? propertyManagerId = null,
        string? tenant = null,
        string? owner = null,
        string? suburb = null,
        bool? isActive = null)
        {
            var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId);

            var (properties, totalCount) = await _unitOfWork.Properties.GetPagedAsync(
                pageNumber: pageNumber,
                pageSize: pageSize,
                predicate: p =>
                    p.AgencyId == tenantAgencyId &&
                    (!propertyType.HasValue || p.Type == propertyType) &&
                    (!propertyManagerId.HasValue || p.PropertyManagerId == propertyManagerId.Value) &&
                    (string.IsNullOrWhiteSpace(suburb) || p.CityOrSuburb.Contains(suburb)) &&
                    (!isActive.HasValue || p.IsActive == isActive.Value) &&
                    (string.IsNullOrWhiteSpace(tenant) || p.Tenancies.Any(t => t.FullName.Contains(tenant))) &&
                    (string.IsNullOrWhiteSpace(owner) || p.Landlords.Any(l => l.Name.Contains(owner))),
                include: q => q
                    .Include(p => p.PropertyManager)
                    .Include(p => p.PropertyLayout)
                        .ThenInclude(l => l.Areas)
                            .ThenInclude(a => a.Items)
                    .Include(p => p.Tenancies)
                        .ThenInclude(t => t.Tenants)
                    .Include(p => p.Landlords),
                orderBy: q => q.OrderBy(p => p.Address1));

            var propertyDtos = _mapper.Map<List<PropertyDto>>(properties);

            return (propertyDtos, totalCount);
        }


        public async Task<PropertyDto?> GetByIdAsync(Guid propertyId , Guid? agencyId)
        {
            var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId);

            var property = await _unitOfWork.Properties.FirstOrDefaultAsync(
                p => p.Id == propertyId && p.AgencyId == tenantAgencyId,
                include: q => q
                    .Include(p => p.PropertyManager)
                    .Include(p => p.PropertyLayout)
                    .Include(p => p.Landlords)
                    .Include(p => p.Tenancies)
                        .ThenInclude(t => t.Tenants));


            if (property == null)
                return null;

            return _mapper.Map<PropertyDto>(property);
        }


        public async Task<PropertyDto> CreateAsync(PropertyDto propertyDto)
        {
            var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(propertyDto.AgencyId);
            var nowUtc = DateTime.UtcNow;
            var property = _mapper.Map<Property>(propertyDto);
            property.AgencyId = tenantAgencyId;
            property.CreatedAt = nowUtc;
            property.UpdatedAt = nowUtc;

            property.Landlords = _mapper.Map<List<Landlord>>(propertyDto.Landlords);
            foreach (var landlord in property.Landlords)
            {
                landlord.CreatedAt = nowUtc;
                landlord.UpdatedAt = nowUtc;
            }

            property.Tenancies = _mapper.Map<List<Tenancy>>(propertyDto.Tenancies);
            foreach (var tenancy in property.Tenancies)
            {
                tenancy.CreatedAt = nowUtc;
                tenancy.UpdatedAt = nowUtc;
                foreach (var tenant in tenancy.Tenants)
                {
                    tenant.CreatedAt = nowUtc;
                    tenant.UpdatedAt = nowUtc;
                }
            }

            await _unitOfWork.Properties.AddAsync(property);
            try
            {
                await _unitOfWork.CommitAsync();
            }
            catch(Exception ex)
            {
                // Log the exception (you can use a logging framework like Serilog, NLog, etc.)
                Console.WriteLine($"Error creating property: {ex.Message}");
                throw; // Re-throw the exception after logging
            }

            // Map back to DTO
            return _mapper.Map<PropertyDto>(property);
        }


        public async Task<PropertyDto?> UpdateAsync(PropertyDto propertyDto)
        {
            var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(propertyDto.AgencyId);

            // Get existing property including related entities
            var property = await _unitOfWork.Properties
                .FirstOrDefaultAsync(
                    p => p.Id == propertyDto.Id && p.AgencyId == tenantAgencyId,
                    include: q => q
                        .Include(p => p.Landlords)
                        .Include(p => p.Tenancies)
                            .ThenInclude(t => t.Tenants));

            if (property == null) return null;

            _mapper.Map(propertyDto, property);
            property.UpdatedAt = DateTime.UtcNow;

            // --- Landlords ---
            // Remove deleted landlords
            var existingLandlordIds = property.Landlords.Select(l => l.Id).ToList();
            var updatedLandlordIds = propertyDto.Landlords.Select(l => l.Id).Where(id => id != Guid.Empty).ToList();

            foreach (var l in property.Landlords.ToList())
            {
                if (!updatedLandlordIds.Contains(l.Id))
                    property.Landlords.Remove(l);
            }

            // Add or update landlords
            foreach (var lDto in propertyDto.Landlords)
            {
                if (lDto.Id != Guid.Empty)
                {
                    var landlord = property.Landlords.FirstOrDefault(x => x.Id == lDto.Id);
                    if (landlord != null)
                    {
                        landlord.Name = lDto.Name;
                        landlord.Email = lDto.Email;
                        landlord.Phone = lDto.Phone;
                        landlord.UpdatedAt = DateTime.UtcNow;
                    }
                }
                else
                {
                    property.Landlords.Add(new Landlord
                    {
                        Name = lDto.Name,
                        Email = lDto.Email,
                        Phone = lDto.Phone,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
            }

            // --- Tenancies ---
            var existingTenancyIds = property.Tenancies.Select(t => t.Id).ToList();
            var updatedTenancyIds = propertyDto.Tenancies.Select(t => t.Id).Where(id => id != Guid.Empty).ToList();

            foreach (var t in property.Tenancies.ToList())
            {
                if (!updatedTenancyIds.Contains(t.Id))
                    property.Tenancies.Remove(t);
            }

            foreach (var tDto in propertyDto.Tenancies)
            {
                if (tDto.Id != Guid.Empty)
                {
                    var tenancy = property.Tenancies.FirstOrDefault(x => x.Id == tDto.Id);
                    if (tenancy != null)
                    {
                        tenancy.FullName = tDto.FullName;
                        tenancy.Email = tDto.Email;
                        tenancy.Mobile = tDto.Mobile;
                        tenancy.LeaseStartDate = DateTime.SpecifyKind(tDto.LeaseStartDate, DateTimeKind.Utc);
                        tenancy.LeaseEndDate = DateTime.SpecifyKind(tDto.LeaseEndDate, DateTimeKind.Utc);
                        tenancy.CurrentRentAmount = tDto.CurrentRentAmount;
                        tenancy.RentFrequency = tDto.RentFrequency;
                        tenancy.IsActive = tDto.IsActive;
                        tenancy.UpdatedAt = DateTime.UtcNow;

                        // --- Tenants ---
                        var existingTenantIds = tenancy.Tenants.Select(te => te.Id).ToList();
                        var updatedTenantIds = tDto.Tenants.Select(te => te.Id).Where(id => id != Guid.Empty).ToList();

                        foreach (var te in tenancy.Tenants.ToList())
                        {
                            if (!updatedTenantIds.Contains(te.Id))
                                tenancy.Tenants.Remove(te);
                        }

                        foreach (var teDto in tDto.Tenants)
                        {
                            if (teDto.Id != Guid.Empty)
                            {
                                var tenant = tenancy.Tenants.FirstOrDefault(x => x.Id == teDto.Id);
                                if (tenant != null)
                                {
                                    tenant.FirstName = teDto.FirstName;
                                    tenant.LastName = teDto.LastName;
                                    tenant.Email = teDto.Email;
                                    tenant.Phone = teDto.Phone;
                                    tenant.UpdatedAt = DateTime.UtcNow;
                                }
                            }
                            else
                            {
                                tenancy.Tenants.Add(new Tenant
                                {
                                    FirstName = teDto.FirstName,
                                    LastName = teDto.LastName,
                                    Email = teDto.Email,
                                    Phone = teDto.Phone,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow
                                });
                            }
                        }
                    }
                }
                else
                {
                    // New Tenancy
                    var newTenancy = new Tenancy
                    {
                        FullName = tDto.FullName,
                        Email = tDto.Email,
                        Mobile = tDto.Mobile,
                        LeaseStartDate = DateTime.SpecifyKind(tDto.LeaseStartDate, DateTimeKind.Utc),
                        LeaseEndDate = DateTime.SpecifyKind(tDto.LeaseEndDate, DateTimeKind.Utc),
                        CurrentRentAmount = tDto.CurrentRentAmount,
                        RentFrequency = tDto.RentFrequency,
                        IsActive = tDto.IsActive,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    // Add tenants
                    foreach (var teDto in tDto.Tenants)
                    {
                        newTenancy.Tenants.Add(new Tenant
                        {
                            FirstName = teDto.FirstName,
                            LastName = teDto.LastName,
                            Email = teDto.Email,
                            Phone = teDto.Phone,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        });
                    }

                    property.Tenancies.Add(newTenancy);
                }
            }

            // Save all changes in UnitOfWork
            await _unitOfWork.Properties.UpdateAsync(property);
            await _unitOfWork.CommitAsync();

            // Return updated property as DTO
            return _mapper.Map<PropertyDto>(property);
        }


        public async Task<bool> DeletePropertyAsync(Guid propertyId , Guid? agencyId)
        {
            var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId);
            var property = await _unitOfWork.Properties.FirstOrDefaultAsync(
                p => p.Id == propertyId
                && p.AgencyId == tenantAgencyId
            );
            if (property == null) return false;

            _unitOfWork.Properties.Remove(property);
            await _unitOfWork.CommitAsync();

            return true;
        }
    }
}
