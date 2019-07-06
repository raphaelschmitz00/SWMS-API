using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;


namespace SwmsApi.Infrastructure
{
	public static class SwmsCorsInstaller
	{
		private static readonly string PolicyName = "CorsPolicy";


		public static void AddSwmsCorsPolicy(this IServiceCollection services)
		{
			services.AddCors(options =>
			{
				options.AddPolicy(PolicyName,
					builder => builder.AllowAnyOrigin()
						.AllowAnyMethod()
						.AllowAnyHeader()
						.AllowCredentials());
			});
		}


		public static void UseSwmsCors(this IApplicationBuilder app)
		{
			app.UseCors(PolicyName);
		}
	}
}