using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace SwmsApi.Infrastructure.Emails
{
	public static class SwmsEmailServiceExtensions
	{
		public static void AddSwmsEmail(this IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
			services.AddSingleton<IEmailSender, EmailSender>();
		}
	}
}