using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace SwmsApi
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}


		public IConfiguration Configuration { get; }


		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDbContext<SwmsContext>(options =>
				options.UseSqlServer(Configuration.GetConnectionString("SwmsContext")));

			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

			services.AddCors(options =>
			{
				options.AddPolicy("CorsPolicy",
					builder => builder.AllowAnyOrigin()
						.AllowAnyMethod()
						.AllowAnyHeader()
						.AllowCredentials());
			});
		}


		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseCors("CorsPolicy");


			app.UseMvc();
		}
	}
}