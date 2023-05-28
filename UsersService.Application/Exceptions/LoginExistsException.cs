using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersService.Application.Exceptions
{
    public class LoginExistsException : Exception
    {
        public LoginExistsException(string login)
            : base($"User with login \"{login}\" already exists.") { }
    }
}
