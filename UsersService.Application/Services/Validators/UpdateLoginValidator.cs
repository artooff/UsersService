using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersService.Application.DTO;

namespace UsersService.Application.Services.Validators
{
    public class UpdateLoginValidator : AbstractValidator<UpdateLoginDto>
    {
        public UpdateLoginValidator()
        {
            RuleFor(x => x.NewLogin)
            .NotEmpty()
            .Matches("^[a-zA-Z0-9]+$")
            .WithMessage("Login can only contain Latin letters and numbers");
        }
    }
}
