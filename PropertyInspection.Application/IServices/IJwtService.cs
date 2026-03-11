using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Application.IServices
{
    public interface IJwtService
    {
        string GenerateJwtToken(IEnumerable<Claim> claims);
    }
}
