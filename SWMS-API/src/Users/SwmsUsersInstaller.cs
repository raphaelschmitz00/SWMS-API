using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SwmsApi.Infrastructure;
using SwmsApi.Infrastructure.Databases;
using SwmsApi.Users.Authorization;
using SwmsApi.Users.Controllers;
using SwmsApi.Users.EmailConfirmation;


namespace SwmsApi.Users
{
	public static class SwmsUsersInstaller
	{
		public static void AddSwmsUsers(this IServiceCollection services, IConfiguration configuration)
		{
			RegisterUserModel(services);
			IConfigurationSection jwtSettingsSection = configuration.GetSection("JwtSettings");
			RegisterNecessaryInterfaces(services, jwtSettingsSection);
			AuthenticationBuilder authenticationBuilder = AddAuthentication(services);
			JwtSettings jwtSettings = jwtSettingsSection.Get<JwtSettings>();
			AddJwtBearer(jwtSettings, authenticationBuilder);
		}


		private static void RegisterUserModel(IServiceCollection services)
		{
			services.AddIdentity<SwmsUser, IdentityRole<long>>()
				.AddEntityFrameworkStores<SwmsContext>()
				.AddDefaultTokenProviders();
		}


		private static void RegisterNecessaryInterfaces(IServiceCollection services, IConfigurationSection jwtSettingsSection)
		{
			services.Configure<JwtSettings>(jwtSettingsSection);
			services.AddSingleton<IJwtGenerator, JwtGenerator>();
			services.AddSingleton<ISwmsAuthorizer, SwmsAuthorizer>();
			services.AddSingleton<IUserEmailConfirmer, UserEmailConfirmer>();
		}


		private static AuthenticationBuilder AddAuthentication(IServiceCollection services)
		{
			JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
			return services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			});
		}


		private static void AddJwtBearer(JwtSettings jwtSettings, AuthenticationBuilder authenticationBuilder)
		{
			string issuer = jwtSettings.Issuer;
			string audience = jwtSettings.Issuer;
			string secret = jwtSettings.Secret;

			authenticationBuilder.AddJwtBearer(options =>
			{
				options.RequireHttpsMetadata = false;
				options.SaveToken = true;
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidIssuer = issuer,
					ValidAudience = audience,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret)),
					ClockSkew = TimeSpan.Zero
				};
			});
		}
	}
}