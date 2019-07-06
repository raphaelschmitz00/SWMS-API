using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SwmsApi.Infrastructure.Databases;
using SwmsApi.Infrastructure.Emails;
using SwmsApi.Users;


namespace SwmsApi.Infrastructure
{
	public class Startup
	{
		private readonly ILoggerFactory _loggerFactory;


		public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
		{
			Configuration = configuration;
			_loggerFactory = loggerFactory;
		}


		public IConfiguration Configuration { get; }


		public void ConfigureServices(IServiceCollection services)
		{
			ILogger<Startup> logger = _loggerFactory.CreateLogger<Startup>();
			services.AddSingleton<ILogger>(logger);

			services.AddSwmsDatabase(Configuration);
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

			services.AddSwmsCorsPolicy();
			services.AddSwmsEmail(Configuration);
			services.AddSwmsUsers(Configuration);
		}


		public void Configure(IApplicationBuilder app, IHostingEnvironment env, SwmsContext swmsContext)
		{
			swmsContext.Database.Migrate();

			if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
			else app.UseHsts();


			app.UseHttpsRedirection();
			app.UseSwmsCors();
			app.UseAuthentication();

			app.UseMvc();
		}
	}
}