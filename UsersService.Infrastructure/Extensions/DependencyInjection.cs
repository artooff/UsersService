using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UsersService.Application.Interfaces;
using UsersService.Infrastructure.Persistance;
using UsersService.Infrastructure.Repository;

namespace UsersService.Infrastructure.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection ConfigurePersistance(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<UsersDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("UsersServiceDb"),
                b => b.MigrationsAssembly(typeof(UsersDbContext).Assembly.FullName)), ServiceLifetime.Transient);

            services.AddTransient<IUsersRepository, UsersRepository>();

            return services;
        }
    }
}
