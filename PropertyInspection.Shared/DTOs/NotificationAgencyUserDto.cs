using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Shared.DTOs
{
    public class NotificationAgencyUserDto
    {
        public Guid AgencyId { get; set; }
        public string AgencyName { get; set; } = string.Empty;
        public List<AgencyUserDto> Users { get; set; } = new List<AgencyUserDto>();
    }


    public class AgencyUserDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Fullname { get; set; } = string.Empty;
    }

}
