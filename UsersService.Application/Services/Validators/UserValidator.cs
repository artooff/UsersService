using FluentValidation;
using UsersService.Application.DTO;

namespace UsersService.Application.Services.Validators
{
    public class UserValidator : AbstractValidator<AddUserDto>
    {
        public UserValidator()
        {
            RuleFor(x => x.Login)
                .NotEmpty()
                .Matches("^[a-zA-Z0-9]+$")
                .WithMessage("Login can only contain Latin letters and numbers");
            RuleFor(x => x.Password)
                .NotEmpty()
                .Matches("^[a-zA-Z0-9]+$")
                .WithMessage("Password can only contain Latin letters and numbers"); ;
            RuleFor(x => x.Name)
                .NotEmpty()
                .Matches("^[a-zA-Z0-9]+$")
                .WithMessage("Name can only contain Latin letters and numbers");
            RuleFor(x => x.Gender)
                .NotEmpty()
                .InclusiveBetween(0, 2)
                .WithMessage("Gender value should be between 0 and 2");

        }
    }
}
