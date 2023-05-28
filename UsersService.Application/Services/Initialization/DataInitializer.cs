using UsersService.Application.Interfaces;
using UsersService.Domain.Models;

namespace UsersService.Application.Services.Initialization
{
    public class DataInitializer : IDataInitializer
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IPasswordHashService _passwordHashService;

        public DataInitializer(IUsersRepository usersRepository, IPasswordHashService passwordHashService)
        {
            _usersRepository = usersRepository;
            _passwordHashService = passwordHashService;
        }
        public async Task InitializeData()
        {
            if (!await _usersRepository.AdminExists())
            {
                var admin = new User
                {
                    Login = "admin",
                    Password = "admin",
                    Name = "admin",
                    Gender = 0,
                    IsAdmin = true,
                    CreatedOn = DateTime.Now.ToUniversalTime(),
                    CreatedBy = "initialization"
                };

                admin.Password = _passwordHashService.HashPassword(admin.Password);

                await _usersRepository.AddUser(admin);
            }
        }
    }
}
