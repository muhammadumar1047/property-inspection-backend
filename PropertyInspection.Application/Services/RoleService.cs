using AutoMapper;
using PropertyInspection.Application.IServices;
using PropertyInspection.Core.Entities;
using PropertyInspection.Core.Interfaces.UnitOfWork;
using PropertyInspection.Shared;
using PropertyInspection.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Application.Services
{
    
        public class RoleService : IRoleService
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly ITenantAgencyResolver _tenantAgencyResolver;
            private readonly IMapper _mapper;

            public RoleService(IUnitOfWork unitOfWork, ITenantAgencyResolver tenantAgencyResolver, IMapper mapper)
            {
                _unitOfWork = unitOfWork;
                _tenantAgencyResolver = tenantAgencyResolver;
                _mapper = mapper;
            }

            public async Task<ServiceResponse<List<RoleDto>>> GetRolesAsync(Guid? agencyId)
            {
                try
                {
                    var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId);

                    Expression<Func<Role, bool>> predicate = r =>
                        !r.IsDeleted &&
                        r.AgencyId == tenantAgencyId;

                    var roles = await _unitOfWork.Roles.GetAsync(
                        predicate: predicate,
                        orderBy: q => q.OrderBy(r => r.Name)
                    );

                    var roleDtos = _mapper.Map<List<RoleDto>>(roles);

                    return new ServiceResponse<List<RoleDto>>
                    {
                        Success = true,
                        Message = "Roles retrieved successfully",
                        Data = roleDtos
                    };
                }
                catch
                {
                    return new ServiceResponse<List<RoleDto>>
                    {
                        Success = false,
                        Message = "Unable to process the request at the moment",
                        ErrorCode = ServiceErrorCodes.ServerError
                    };
                }
            }

            public async Task<ServiceResponse<RoleDto>> GetByIdAsync(Guid id)
            {
                try
                {
                    var role = await _unitOfWork.Roles.GetByIdAsync(id);

                    if (role == null || role.IsDeleted)
                    {
                        return new ServiceResponse<RoleDto>
                        {
                            Success = false,
                            Message = "Role not found",
                            ErrorCode = ServiceErrorCodes.NotFound
                        };
                    }

                    return new ServiceResponse<RoleDto>
                    {
                        Success = true,
                        Message = "Role retrieved successfully",
                        Data = _mapper.Map<RoleDto>(role)
                    };
                }
                catch
                {
                    return new ServiceResponse<RoleDto>
                    {
                        Success = false,
                        Message = "Unable to process the request at the moment",
                        ErrorCode = ServiceErrorCodes.ServerError
                    };
                }
            }

            public async Task<ServiceResponse<RoleDto>> CreateAsync(CreateRoleRequest dto, Guid userId)
            {
                try
                {
                    if (dto == null)
                    {
                        return new ServiceResponse<RoleDto>
                        {
                            Success = false,
                            Message = "Invalid request data",
                            ErrorCode = ServiceErrorCodes.InvalidRequest
                        };
                    }

                    var role = new Role
                    {
                        Id = Guid.NewGuid(),
                        Name = dto.Name,
                        Description = dto.Description,
                        AgencyId = _tenantAgencyResolver.ResolveAgencyId(dto.AgencyId),
                        CreatedBy = userId,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _unitOfWork.Roles.AddAsync(role);
                    await _unitOfWork.CommitAsync();

                    return new ServiceResponse<RoleDto>
                    {
                        Success = true,
                        Message = "Role created successfully",
                        Data = _mapper.Map<RoleDto>(role)
                    };
                }
                catch
                {
                    return new ServiceResponse<RoleDto>
                    {
                        Success = false,
                        Message = "Unable to process the request at the moment",
                        ErrorCode = ServiceErrorCodes.ServerError
                    };
                }
            }

            public async Task<ServiceResponse<bool>> UpdateAsync(Guid id, UpdateRoleRequest dto, Guid userId)
            {
                try
                {
                    var role = await _unitOfWork.Roles.GetByIdAsync(id);

                    if (role == null || role.IsDeleted)
                    {
                        return new ServiceResponse<bool>
                        {
                            Success = false,
                            Message = "Role not found",
                            ErrorCode = ServiceErrorCodes.NotFound
                        };
                    }

                    role.Name = dto.Name;
                    role.Description = dto.Description;
                    role.UpdatedBy = userId;
                    role.UpdatedAt = DateTime.UtcNow;

                    await _unitOfWork.CommitAsync();

                    return new ServiceResponse<bool>
                    {
                        Success = true,
                        Message = "Role updated successfully",
                        Data = true
                    };
                }
                catch
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Unable to process the request at the moment",
                        ErrorCode = ServiceErrorCodes.ServerError
                    };
                }
            }

            public async Task<ServiceResponse<bool>> DeleteAsync(Guid id, Guid userId)
            {
                try
                {
                    var role = await _unitOfWork.Roles.GetByIdAsync(id);

                    if (role == null || role.IsDeleted)
                    {
                        return new ServiceResponse<bool>
                        {
                            Success = false,
                            Message = "Role not found",
                            ErrorCode = ServiceErrorCodes.NotFound
                        };
                    }

                    role.IsDeleted = true;
                    role.UpdatedBy = userId;
                    role.UpdatedAt = DateTime.UtcNow;

                    await _unitOfWork.CommitAsync();

                    return new ServiceResponse<bool>
                    {
                        Success = true,
                        Message = "Role deleted successfully",
                        Data = true
                    };
                }
                catch
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Unable to process the request at the moment",
                        ErrorCode = ServiceErrorCodes.ServerError
                    };
                }
            }
        }
    }

