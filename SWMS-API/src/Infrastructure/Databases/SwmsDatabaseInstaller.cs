using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace SwmsApi.Infrastructure.Databases
{
	public static class SwmsDatabaseInstaller
	{
		public static void AddSwmsDatabase(this IServiceCollection services, IConfiguration configuration)
		{
			IConfigurationSection connectionStringsSection = configuration.GetSection("ConnectionStrings");
			ConnectionStrings connectionStrings = connectionStringsSection.Get<ConnectionStrings>();
			services.AddDbContext<SwmsContext>(options =>
					options.UseSqlServer(connectionStrings.SwmsContext)
			);
		}
	}
}