using PropertyInspection.Shared;
using PropertyInspection.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Application.IServices
{
    public interface IRoleService
    {
        Task<ServiceResponse<List<RoleDto>>> GetRolesAsync(Guid? agencyId);

        Task<ServiceResponse<RoleDto>> GetByIdAsync(Guid id);

        Task<ServiceResponse<RoleDto>> CreateAsync(CreateRoleRequest dto, Guid userId);

        Task<ServiceResponse<bool>> UpdateAsync(Guid id, UpdateRoleRequest dto, Guid userId);

        Task<ServiceResponse<bool>> DeleteAsync(Guid id, Guid userId);
    }
}
