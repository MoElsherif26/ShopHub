using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopHub.Core.DTO
{
    public record LoginDTO
    {
        public string Eamil { get; set; }
        public string Password { get; set; }

    }
    public record RegisterDTO: LoginDTO
    {
        public string UserName { get; set; }
    }
}
