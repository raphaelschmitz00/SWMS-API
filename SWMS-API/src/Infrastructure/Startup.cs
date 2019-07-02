using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SwmsApi.Users;
using SwmsApi.Users.Controllers;


namespace SwmsApi.Infrastructure
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


			services.AddIdentity<SwmsUser, IdentityRole<long>>()
				.AddEntityFrameworkStores<SwmsContext>()
				.AddDefaultTokenProviders();

			IConfigurationSection appSettingsSection = Configuration.GetSection("AppSettings");
			services.Configure<AppSettings>(appSettingsSection);
			AppSettings appSettings = appSettingsSection.Get<AppSettings>();
			byte[] key = Encoding.ASCII.GetBytes(appSettings.Secret);
			services.AddAuthentication(x =>
				{
					x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
					x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				})
				.AddJwtBearer(x =>
				{
					x.RequireHttpsMetadata = false;
					x.SaveToken = true;
					x.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuerSigningKey = true,
						IssuerSigningKey = new SymmetricSecurityKey(key),
						ValidateIssuer = false,
						ValidateAudience = false
					};
				});

			services.AddScoped<IPasswordHasher, PasswordHasher>();
			services.AddScoped<IJwtFactory, JwtFactory>();

		}


		public void Configure(IApplicationBuilder app, IHostingEnvironment env, SwmsContext swmsContext)
		{
			swmsContext.Database.Migrate();

			if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
			else app.UseHsts();


			app.UseHttpsRedirection();
			app.UseCors("CorsPolicy");
			app.UseAuthentication();
			

			app.UseMvc();
		}
	}
}