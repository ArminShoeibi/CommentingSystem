using CommentingSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace CommentingSystem.Web.Configurations;

public static class DbContextConfig
{
    public static void AddDbContextConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configuration is null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        services.AddDbContext<CommentingSystemDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
    }
}