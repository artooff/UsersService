using FluentValidation;
using UsersService.Application.DTO;

namespace UsersService.Application.Services.Validators
{
    public class UpdateDetailsValidator : AbstractValidator<UpdateDetailsDto>
    {
        public UpdateDetailsValidator()
        {
            RuleFor(x => x.Name)
                .Matches("^[a-zA-Z0-9]+$")
                .WithMessage("Name can only contain Latin letters and numbers");

            RuleFor(x => x.Gender)
                .InclusiveBetween(0, 2)
                .WithMessage("Gender value should be between 0 and 2");


        }
    }
}
