using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersService.Application.DTO
{
    public record UpdateLoginDto
    {
        public string Login { get; init; }
        public string NewLogin { get; init; }
        public string ModifiedBy { get; init; }
        public DateTime ModifiedOn { get; init; }
    }
}
