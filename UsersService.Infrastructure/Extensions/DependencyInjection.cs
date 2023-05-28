using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using UsersService.Application.Interfaces;
using UsersService.Infrastructure.Persistance;
using UsersService.Infrastructure.Repository;

namespace UsersService.Infrastructure.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection ConfigurePersistance(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<UsersDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("UsersDbConnection"),
                    b => b.MigrationsAssembly(typeof(UsersDbContext).Assembly.FullName));
            }, ServiceLifetime.Transient);

            services.AddTransient<IUsersRepository, UsersRepository>();

            return services;
        }

        public static IServiceCollection ConfigureSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition(configuration["Swagger:SecurityDefinitionName"], new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = configuration["Swagger:SecurityDefinitionDescription"],
                    In = Enum.Parse<ParameterLocation>(configuration["Swagger:SecuritySchemeIn"]),
                    Type = Enum.Parse<SecuritySchemeType>(configuration["Swagger:SecuritySchemeType"]),
                    Scheme = configuration["Swagger:SecuritySchemeScheme"]
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Name = "Bearer",
                            In = Enum.Parse<ParameterLocation>(configuration["Swagger:SecuritySchemeIn"]),
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new List<string>()
                    }
                });
            });

            return services;
        }
    }
}
