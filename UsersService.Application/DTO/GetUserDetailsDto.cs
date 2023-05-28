using Newtonsoft.Json;

namespace UsersService.Application.DTO
{
    public record GetUserDetailsDto(string Name,
        int Gender,
        DateTime? Birthday,
        bool IsActive);

}
