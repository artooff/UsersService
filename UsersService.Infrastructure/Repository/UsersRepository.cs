
using UsersService.Application.DTO;
using UsersService.Application.Interfaces;
using UsersService.Infrastructure.Persistance;

namespace UsersService.Infrastructure.Repository
{
    public class UsersRepository : IUsersRepository
    {
        private readonly UsersDbContext _dbContext;

        public UsersRepository(UsersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<int> AddUser(AddUserDto userModel)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteUserFull(string login)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteUserSoft(string login)
        {
            throw new NotImplementedException();
        }

        public Task<List<GetUserDto>> GetActiveUsers()
        {
            throw new NotImplementedException();
        }

        public Task<GetUserDetailsDto> GetUserByLogin(string login)
        {
            throw new NotImplementedException();
        }

        public Task<GetUserDetailsDto> GetUserByLoginAndPassword(string login, string password)
        {
            throw new NotImplementedException();
        }

        public Task<List<GetUserDto>> GetUsersByAge()
        {
            throw new NotImplementedException();
        }

        public Task<int> RestoreUser(string login)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateBirthday(DateTime birthday)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateGender(int gender)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateLogin(string login)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateName(string name)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdatePassword(string password)
        {
            throw new NotImplementedException();
        }
    }
}
