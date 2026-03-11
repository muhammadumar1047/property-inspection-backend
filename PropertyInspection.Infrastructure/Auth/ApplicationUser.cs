using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Infrastructure.Auth
{
    public class ApplicationUser : IdentityUser
    {
        public Guid? AgencyId { get; set; }
    }
}
