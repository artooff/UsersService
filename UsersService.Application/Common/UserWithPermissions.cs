using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersService.Application.Common
{
    public record UserWithPermissions(string Login, bool IsAdmin);
}
