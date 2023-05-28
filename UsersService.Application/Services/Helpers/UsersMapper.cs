using AutoMapper;
using UsersService.Application.DTO;
using UsersService.Domain.Models;

namespace UsersService.Application.Services.Helpers
{
    public class UsersMapper : Profile
    {
        public UsersMapper()
        {
            CreateMap<AddUserDto, User>().ReverseMap();
            CreateMap<User, GetUserDto>().ReverseMap();
            CreateMap<User, GetUserDetailsDto>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => !src.RevokedOn.HasValue))
                .ConstructUsing(src => new GetUserDetailsDto(src.Name, src.Gender, src.Birthday, !src.RevokedOn.HasValue)); ;

        }
    }
}
