using Newtonsoft.Json;

namespace UsersService.Application.DTO
{
    public record AddUserDto(string Login,
        string Password,
        string Name,
        int Gender,
        DateTime? Birthday,
        bool IsAdmin);

}
