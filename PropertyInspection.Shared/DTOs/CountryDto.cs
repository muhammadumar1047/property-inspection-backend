using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyInspection.Shared.DTOs
{
    public class CountryDto
    {
        public Guid Id { get; set; }   
        public string Name { get; set; } = null!;
        public string IsoAlpha3 { get; set; } = null!;
        public string IsoAlpha2 { get; set; } = null!;
    }
}
