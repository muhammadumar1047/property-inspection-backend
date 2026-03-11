using Microsoft.EntityFrameworkCore;
using PropertyInspection.Application.IServices;
using PropertyInspection.Core.Entities;
using PropertyInspection.Core.Interfaces.UnitOfWork;
using PropertyInspection.Infrastructure.Auth;
using PropertyInspection.Shared;
using PropertyInspection.Shared.Auth;
using PropertyInspection.Shared.DTOs;
using AutoMapper;
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

        public async Task<PagedResult<UserDto>> GetUsersAsync(
            UserFilterDto filter,
            Guid? agencyId,
            int page = 1,
            int pageSize = 10)
        {
            int currentPage = page < 1 ? 1 : page;
            int skip = (currentPage - 1) * pageSize;

            var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId);

            Expression<Func<User, bool>> predicate = u =>
                !u.IsDeleted &&
                u.AgencyId == tenantAgencyId &&
                (string.IsNullOrEmpty(filter.Search)
                    || u.FirstName.Contains(filter.Search)
                    || u.LastName.Contains(filter.Search)
                    || u.Email.Contains(filter.Search)) &&
                (!filter.DepartmentId.HasValue) &&
                (!filter.IsActive.HasValue || u.IsDeleted != filter.IsActive.Value) &&
                (!filter.RoleId.HasValue || u.UserRoles.Any(ur => ur.RoleId == filter.RoleId));

            var users = await _unitOfWork.Users.GetAsync(
                predicate: predicate,
                include: q => q
                    .Include(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role),
                orderBy: q => q.OrderBy(u => u.FirstName),
                skip: skip,
                take: pageSize
            );

            var totalCount = await _unitOfWork.Users.CountAsync(predicate);

            var userDtos = _mapper.Map<List<UserDto>>(users);

            return new PagedResult<UserDto>
            {
                Data = userDtos,
                Page = currentPage,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<UserDto> GetByIdAsync(Guid id)
        {
            //var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(agencyId);
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
                throw new Exception("User not found");

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> CreateAsync(CreateUserDto dto)
        {

            var tenantAgencyId = _tenantAgencyResolver.ResolveAgencyId(dto.AgencyId);

            var identityUser = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                EmailConfirmed = true,
                AgencyId = tenantAgencyId,
            };
            var identityResult = await _userAuthService.CreateUserAsync(identityUser, dto.Password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                IdentityUserId = identityUser.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                AgencyId = tenantAgencyId,
                CreatedBy = Guid.Parse(_tenant.DomainUserId ?? ""),
                CreatedAt = DateTime.UtcNow.Date,
                IsAgencyAdmin = dto.IsAgencyAdmin ?? false,
                UserRoles = dto.RoleIds.Distinct().Select(roleId => new UserRole
                {
                    RoleId = roleId
                }).ToList()
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task UpdateAsync(Guid id, UpdateUserDto dto, Guid userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(
                id,
                include: q => q.Include(u => u.UserRoles));

            if (user == null)
                throw new Exception("User not found");

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
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
        }

        public async Task DeleteAsync(Guid id, Guid userId)
        {
            await _unitOfWork.Users.DeleteAsync(id, userId);
            await _unitOfWork.CommitAsync();
        }

        public async Task<UserDto?> GetUserWithRolesByIdentityIdAsync(string identityUserId)
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
                return null;

            return _mapper.Map<UserDto>(user);
        }

       
    }
}
