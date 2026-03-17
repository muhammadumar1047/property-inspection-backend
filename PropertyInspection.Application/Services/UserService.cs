using Microsoft.EntityFrameworkCore;
using PropertyInspection.Application.IServices;
using PropertyInspection.Core.Entities;
using PropertyInspection.Core.Interfaces.UnitOfWork;
using PropertyInspection.Infrastructure.Auth;
using PropertyInspection.Shared;
using PropertyInspection.Shared.Auth;
using PropertyInspection.Shared.DTOs;
using AutoMapper;
using PropertyInspection.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PropertyInspection.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserAuthService _userAuthService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantContext _tenant;
        private readonly ITenantAgencyResolver _tenantAgencyResolver;
        private readonly IMapper _mapper;

        public UserService(
            IUserAuthService userAuthService,
            IUnitOfWork unitOfWork,
            ITenantContext tenant,
            ITenantAgencyResolver tenantAgencyResolver,
            IMapper mapper)
        {
            _userAuthService = userAuthService;
            _unitOfWork = unitOfWork;
            _tenant = tenant;
            _tenantAgencyResolver = tenantAgencyResolver;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<PagedResult<UserResponse>>> GetUsersAsync(
            UserFilterDto filter,
            Guid? agencyId,
            int page = 1,
            int pageSize = 10)
        {
            try
            {
                int currentPage = page < 1 ? 1 : page;
                int skip = (currentPage - 1) * pageSize;

                var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId);

                Expression<Func<User, bool>> predicate = u =>
                    !u.IsDeleted &&
                    u.AgencyId == tenantAgencyId &&
                    (string.IsNullOrEmpty(filter.Search)
                        || (u.FirstName ?? string.Empty).Contains(filter.Search)
                        || (u.LastName ?? string.Empty).Contains(filter.Search)
                        || (u.Email ?? string.Empty).Contains(filter.Search)) &&
                    (!filter.DepartmentId.HasValue) &&
                    (!filter.IsActive.HasValue || u.IsActive == filter.IsActive.Value) &&
                    (!filter.RoleId.HasValue || u.UserRoles.Any(ur => ur.RoleId == filter.RoleId.Value));

                var users = await _unitOfWork.Users.GetAsync(
                    predicate: predicate,
                    include: q => q
                        .Include(u => u.Agency)
                        .Include(u => u.UserRoles)
                            .ThenInclude(ur => ur.Role)
                                .ThenInclude(r => r.RolePermissions)
                                    .ThenInclude(rp => rp.Permission),
                    orderBy: q => q.OrderBy(u => u.FirstName),
                    skip: skip,
                    take: pageSize
                );

                var totalCount = await _unitOfWork.Users.CountAsync(predicate);

                var userDtos = _mapper.Map<List<UserResponse>>(users);
                var result = new PagedResult<UserResponse>
                {
                    Data = userDtos,
                    Page = currentPage,
                    PageSize = pageSize,
                    TotalCount = totalCount
                };

                return new ServiceResponse<PagedResult<UserResponse>>
                {
                    Success = true,
                    Message = "Records retrieved successfully",
                    Data = result
                };
            }
            catch
            {
                return new ServiceResponse<PagedResult<UserResponse>>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<UserResponse>> GetByIdAsync(Guid id)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(
                    id,
                    include: q => q
                        .Include(u => u.Agency)
                        .Include(u => u.UserRoles)
                            .ThenInclude(ur => ur.Role)
                                .ThenInclude(r => r.RolePermissions)
                                    .ThenInclude(rp => rp.Permission)
                );

                if (user == null)
                {
                    return new ServiceResponse<UserResponse>
                    {
                        Success = false,
                        Message = "Record not found",
                        ErrorCode = ServiceErrorCodes.NotFound
                    };
                }

                return new ServiceResponse<UserResponse>
                {
                    Success = true,
                    Message = "Record retrieved successfully",
                    Data = _mapper.Map<UserResponse>(user)
                };
            }
            catch
            {
                return new ServiceResponse<UserResponse>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<UserResponse>> CreateAsync(CreateUserRequest dto)
        {
            try
            {
                if (dto == null)
                {
                    return new ServiceResponse<UserResponse>
                    {
                        Success = false,
                        Message = "Invalid request data",
                        ErrorCode = ServiceErrorCodes.InvalidRequest
                    };
                }

                var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(dto.AgencyId);

                var identityUser = new ApplicationUser
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                    EmailConfirmed = true,
                    AgencyId = tenantAgencyId,
                };
                var identityResult = await _userAuthService.CreateUserAsync(identityUser, dto.Password);
                if (!identityResult.Success || identityResult.Data == null)
                {
                    return new ServiceResponse<UserResponse>
                    {
                        Success = false,
                        Message = identityResult.Message,
                        ErrorCode = identityResult.ErrorCode ?? ServiceErrorCodes.ServerError
                    };
                }

                identityUser = identityResult.Data;

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    IdentityUserId = identityUser.Id,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    AgencyId = tenantAgencyId,
                    CreatedBy = Guid.Parse(_tenant.DomainUserId ?? Guid.Empty.ToString()),
                    CreatedAt = DateTime.UtcNow,
                    IsAgencyAdmin = dto.IsAgencyAdmin ?? false,
                    UserRoles = (dto.RoleIds ?? new List<Guid>())
                        .Distinct()
                        .Select(roleId => new UserRole
                        {
                            RoleId = roleId
                        }).ToList()
                };

                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.CommitAsync();

                return new ServiceResponse<UserResponse>
                {
                    Success = true,
                    Message = "Entity created successfully",
                    Data = _mapper.Map<UserResponse>(user)
                };
            }
            catch
            {
                return new ServiceResponse<UserResponse>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

        public async Task<ServiceResponse<bool>> UpdateAsync(Guid id, UpdateUserRequest dto, Guid userId)
        {
            try
            {
                if (dto == null)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Invalid request data",
                        ErrorCode = ServiceErrorCodes.InvalidRequest
                    };
                }

                var user = await _unitOfWork.Users.GetByIdAsync(
                    id,
                    include: q => q.Include(u => u.UserRoles));

                if (user == null)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Record not found",
                        ErrorCode = ServiceErrorCodes.NotFound
                    };
                }

                user.FirstName = dto.FirstName;
                user.LastName = dto.LastName;

                if (dto.AgencyId.HasValue)
                {
                    user.AgencyId = dto.AgencyId.Value;
                }
                user.UpdatedBy = userId;
                user.UpdatedAt = DateTime.UtcNow;

                user.UserRoles.Clear();
                foreach (var roleId in dto.RoleIds.Distinct())
                {
                    user.UserRoles.Add(new UserRole
                    {
                        RoleId = roleId
                    });
                }

                await _unitOfWork.CommitAsync();

                return new ServiceResponse<bool>
                {
                    Success = true,
                    Message = "Record updated successfully",
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
                await _unitOfWork.Users.DeleteAsync(id, userId);
                await _unitOfWork.CommitAsync();

                return new ServiceResponse<bool>
                {
                    Success = true,
                    Message = "Record deleted successfully",
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

        public async Task<ServiceResponse<UserResponse>> GetUserWithRolesByIdentityIdAsync(string identityUserId)
        {
            try
            {
                var user = await _unitOfWork.Users.FirstOrDefaultAsync(
                    u => u.IdentityUserId == identityUserId,
                    include: q => q
                        .Include(u => u.Agency)
                        .Include(u => u.UserRoles)
                            .ThenInclude(ur => ur.Role)
                                .ThenInclude(r => r.RolePermissions)
                                    .ThenInclude(rp => rp.Permission)
                );

                if (user == null || user.IsDeleted)
                {
                    return new ServiceResponse<UserResponse>
                    {
                        Success = false,
                        Message = "Record not found",
                        ErrorCode = ServiceErrorCodes.NotFound
                    };
                }

                return new ServiceResponse<UserResponse>
                {
                    Success = true,
                    Message = "Record retrieved successfully",
                    Data = _mapper.Map<UserResponse>(user)
                };
            }
            catch
            {
                return new ServiceResponse<UserResponse>
                {
                    Success = false,
                    Message = "Unable to process the request at the moment",
                    ErrorCode = ServiceErrorCodes.ServerError
                };
            }
        }

       
    }
}

