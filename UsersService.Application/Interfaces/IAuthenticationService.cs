using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersService.Application.Common;

namespace UsersService.Application.Interfaces
{
    public interface IAuthenticationService<T> where T : class
    {
        Task<Result<T>> Authenticate(string login, string password);
    }
}
