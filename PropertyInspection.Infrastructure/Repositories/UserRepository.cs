using Microsoft.EntityFrameworkCore;
using PropertyInspection.Core.Entities;
using PropertyInspection.Core.Interfaces.Repositories;
using PropertyInspection.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        
        public async Task<User?> GetWithRolesAsync(Guid id) =>
           await _dbSet.Include(u => u.UserRoles)
                               .ThenInclude(ur => ur.Role)
                               .FirstOrDefaultAsync(u => u.Id == id);
        public async Task<User?> GetByIdentityUserIdAsync(string identityUserId)
        {
          
            return await _dbSet
               .Include(u => u.Agency)
               .Include(u => u.UserRoles)
                   .ThenInclude(ur => ur.Role)
                       .ThenInclude(r => r.RolePermissions)
                           .ThenInclude(rp => rp.Permission)
               .FirstOrDefaultAsync(u => u.IdentityUserId == identityUserId);
        }

        public async Task<List<User>> GetByAgencyAsync(Guid agencyId) =>
            await _dbSet
                .Where(u => u.AgencyId == agencyId)
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .AsNoTracking()
                .ToListAsync();
    }
}
