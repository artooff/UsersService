using UsersService.Application.DTO;

namespace UsersService.Application.Interfaces
{
    public interface IUsersRepository
    {
        public Task<int> AddUser(AddUserDto userModel);
        public Task<int> UpdateLogin(string login);
        public Task<int> UpdatePassword(string password);
        public Task<int> UpdateName(string name);
        public Task<int> UpdateGender(int gender);
        public Task<int> UpdateBirthday(DateTime birthday);
        public Task<List<GetUserDto>> GetActiveUsers();
        public Task<List<GetUserDto>> GetUsersByAge();
        public Task<GetUserDetailsDto> GetUserByLogin(string login);
        public Task<GetUserDetailsDto> GetUserByLoginAndPassword(string login, string password);
        public Task<int> DeleteUserSoft(string login);
        public Task<int> DeleteUserFull(string login);
        public Task<int> RestoreUser(string login);
    }
}
