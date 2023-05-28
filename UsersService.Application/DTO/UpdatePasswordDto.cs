using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersService.Application.DTO
{
    public record UpdatePasswordDto
    {
        public string Login { get; init; }
        public string NewPassword { get; set; }
        public string ModifiedBy { get; init; }
        public DateTime ModifiedOn { get; init; }
    }
}
