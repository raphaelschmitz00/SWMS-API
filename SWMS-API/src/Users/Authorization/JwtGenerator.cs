using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SwmsApi.Infrastructure;


namespace SwmsApi.Users.Authorization
{
	public class JwtGenerator : IJwtGenerator
	{
		private readonly JwtSettings _jwtSettings;


		public JwtGenerator(IOptions<JwtSettings> jwtSettings)
		{
			_jwtSettings = jwtSettings.Value;
		}


		public object GenerateJwtToken(SwmsUser user)
		{
			List<Claim> claims = new List<Claim>
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.Email),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
			};

			string issuer = _jwtSettings.Issuer;
			string audience = issuer;
			SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
			DateTime expires = DateTime.Now.AddDays(Convert.ToDouble(_jwtSettings.ExpireDays));
			SigningCredentials signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
			JwtSecurityToken token = new JwtSecurityToken(issuer, audience, claims, null, expires, signingCredentials);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}