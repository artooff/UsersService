using Microsoft.AspNetCore.JsonPatch;
using UsersService.Application.Common;
using UsersService.Application.DTO;
using UsersService.Domain.Models;

namespace UsersService.Application.Interfaces
{
    public interface IUsersRepository
    {
        public Task<List<User>> GetUsers();
        public Task<User> GetUser(string login);
        public Task AddUser(User user);
        public Task UpdatePassword(UpdatePasswordDto model, UserWithPermissions currentUser);
        public Task UpdateLogin(UpdateLoginDto model, UserWithPermissions currentUser);
        public Task UpdateDetails(string login, JsonPatchDocument model, UserWithPermissions currentUser);
        public Task<int> DeleteUserSoft(string login, string revokedBy);
        public Task<int> DeleteUserFull(string login);
        public Task<int> RestoreUser(string login);
        public Task<bool> AdminExists();
    }
}
