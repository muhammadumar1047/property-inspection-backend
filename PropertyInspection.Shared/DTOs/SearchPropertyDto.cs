using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Shared.DTOs
{
    public class SearchPropertyDto
    {
        public Guid Id { get; set; }
        public string Address { get; set; } = string.Empty;
    }
}
