using Microsoft.Extensions.DependencyInjection;
using UsersService.Application.Interfaces;
using UsersService.Application.Services.Helpers;
using UsersService.Application.Services.Validators;
using FluentValidation;
using UsersService.Application.DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UsersService.Application.Services;
using UsersService.Application.Common;
using Microsoft.Extensions.Logging;
using UsersService.Application.Services.Initialization;

namespace UsersService.Application.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            services.AddTransient<IValidator<AddUserDto>, UserValidator>();
            services.AddTransient<IValidator<UpdateLoginDto>, UpdateLoginValidator>();
            services.AddTransient<IValidator<UpdatePasswordDto>, UpdatePasswordValidator>();
            services.AddTransient<IValidator<UpdateDetailsDto>, UpdateDetailsValidator>();
            services.AddTransient<IPasswordHashService, BCryptHashService>();
            services.AddTransient<IUsersService, Services.CRUD.UsersService>();
            services.AddTransient<IDataInitializer, DataInitializer>();
            services.AddAutoMapper(typeof(UsersMapper));

            return services;
        }

        public static IServiceCollection ConfigureAuthentication(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                 .AddJwtBearer(options => {
                     options.TokenValidationParameters = new TokenValidationParameters
                     {
                         ValidateIssuer = true,
                         ValidateAudience = true,
                         ValidateLifetime = true,
                         ValidateIssuerSigningKey = true,
                         ValidIssuer = configuration["Jwt:Issuer"],
                         ValidAudience = configuration["Jwt:Audience"],
                         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
                     };
                 });
            services.AddTransient<IAuthenticationService<JwtTokens>, JwtAuthenticationService>();

            services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());

            return services;
        }

    }
}
