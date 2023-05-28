using Microsoft.AspNetCore.JsonPatch;
using UsersService.Application.Common;
using UsersService.Application.DTO;

namespace UsersService.Application.Interfaces
{
    public interface IUsersService
    {
        public Task<Result<List<GetUserDto>>> GetActiveUsers();
        public Task<Result<List<GetUserDto>>> GetUsersByAge(int age);
        public Task<Result<GetUserDetailsDto>> GetUserByLogin(string login);
        public Task<Result<GetUserDetailsDto>> GetCurrentUser(string login);
        public Task<Result<string>> AddUser(AddUserDto userModel, string currentUserLogin);
        public Task<Result<string>> UpdateLogin(string login, string newLogin, UserWithPermissions user);
        public Task<Result<string>> UpdatePassword(string login, string newPassword, UserWithPermissions user);
        public Task<Result<string>> UpdateDetails(string login, JsonPatchDocument patchDocument, UserWithPermissions user);
        public Task<Result<string>> DeleteUserSoft(string login, string currentUserLogin);
        public Task<Result<string>> DeleteUserFull(string login);
        public Task<Result<string>> RestoreUser(string login);

    }
}
