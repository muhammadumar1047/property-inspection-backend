using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Shared
{
    public class JwtSettings
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int TokenExpiryMinutes { get; set; }
        public int TokenExpiryMinutesRememberMe { get; set; }
        public int CookieExpiryHours { get; set; }
        public int CookieExpiryHoursRememberMe { get; set; }
    }
}
