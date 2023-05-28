using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace UsersService.Application.Common
{
    public static class Permissions
    {
        public static UserWithPermissions GetUserWithPermissions(string login, ClaimsPrincipal claims)
        {
            var currentUserLogin = claims.FindFirstValue(ClaimTypes.Name);
            var currentUserRole = claims.FindFirstValue(ClaimTypes.Role);
            if (currentUserLogin == login || currentUserRole == "Admin")
            {
                return new UserWithPermissions(currentUserLogin, currentUserRole == "Admin");
            }
            return null;
        }
    }
}
