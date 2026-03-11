using PropertyInspection.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Core.Interfaces.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<List<User>> GetByAgencyAsync(Guid agencyId);
        Task<User?> GetByIdentityUserIdAsync(string identityUserId);
        Task<User?> GetWithRolesAsync(Guid id);
    }
}
