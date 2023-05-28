using FluentValidation;
using UsersService.Application.DTO;

namespace UsersService.Application.Services.Validators
{
    public class UpdatePasswordValidator : AbstractValidator<UpdatePasswordDto>
    {
        public UpdatePasswordValidator()
        {
            RuleFor(x => x.NewPassword)
            .NotEmpty()
            .Matches("^[a-zA-Z0-9]+$")
            .WithMessage("Password can only contain Latin letters and numbers");
        }
    }
}
