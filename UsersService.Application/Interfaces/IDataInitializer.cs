using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersService.Application.Interfaces
{
    public interface IDataInitializer
    {
        public Task InitializeData();
    }
}
