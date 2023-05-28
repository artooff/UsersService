using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersService.Application.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string login)
            : base($"User with login \"{login}\" was not found.") { }
    }
}
