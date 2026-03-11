using Microsoft.EntityFrameworkCore;
using PropertyInspection.Application.IServices;
using PropertyInspection.Core.Entities;
using PropertyInspection.Core.Interfaces.UnitOfWork;
using PropertyInspection.Shared.Auth;
using PropertyInspection.Shared.DTOs;
using AutoMapper;

namespace PropertyInspection.Application.Services
{
    public class AgencyManagementService : IAgencyManagementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AgencyManagementService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<UserDto> AddUserAsync(Guid actingUserId, Guid actingAgencyId, string? role, AddAgencyUsers dto)
        {
            if (role != "Admin")
                throw new UnauthorizedAccessException("Only Admin can add users.");

            var user = new User
            {
                Email = dto.Email
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<UserDto>(user);
        }

        public async Task<List<UserDto>> GetUsersAsync(Guid agencyId)
        {
            var users = await _unitOfWork.Users.GetAsync(
                predicate: u => u.AgencyId == agencyId,
                include: q => q
                    .Include(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role),
                orderBy: q => q.OrderBy(u => u.FirstName));
            return _mapper.Map<List<UserDto>>(users);
        }

        public async Task<bool> UpdateRoleAsync(Guid targetUserId, Guid actingAgencyId, string? role, UpdateRoleDto dto)
        {
            await Task.CompletedTask;
            return false;
        }

        public async Task<bool> DeleteUserAsync(Guid targetUserId, Guid actingAgencyId, string? role)
        {
            if (role != "Admin")
                throw new UnauthorizedAccessException("Only Admin can delete users.");

            await Task.CompletedTask;
            return true;
        }
    }
}
