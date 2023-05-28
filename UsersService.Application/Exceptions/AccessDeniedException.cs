using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersService.Application.Exceptions
{
    public class AccessDeniedException : Exception
    {
        public AccessDeniedException(string login) 
            : base($"User with login \"{login}\" was revoked.") { }
    }
}
