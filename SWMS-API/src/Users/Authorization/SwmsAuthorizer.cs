using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SwmsApi.Users.Controllers;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;


namespace SwmsApi.Users.Authorization
{
	public class SwmsAuthorizer : ISwmsAuthorizer
	{
		private readonly ILogger _logger;
		private readonly UserManager<SwmsUser> _userManager;
		private readonly SignInManager<SwmsUser> _signInManager;
		private readonly IJwtGenerator _jwtGenerator;


		public SwmsAuthorizer(ILogger logger, UserManager<SwmsUser> userManager, IJwtGenerator jwtGenerator,
			SignInManager<SwmsUser> signInManager)
		{
			_logger = logger;
			_userManager = userManager;
			_jwtGenerator = jwtGenerator;
			_signInManager = signInManager;
		}


		async Task<ActionResult<SwmsUser>> ISwmsAuthorizer.SignUp(ControllerBase controllerBase, SignUpDto signUpDto)
		{
			_logger.LogInformation("Signing up new user");
			IdentityResult identityResult = await CreateAccountAsync(signUpDto);
			if (!identityResult.Succeeded) return controllerBase.BadRequest(identityResult.Errors);
			return controllerBase.Ok();
		}


		private async Task<IdentityResult> CreateAccountAsync(SignUpDto signUpDto)
		{
			SwmsUser swmsUser = new SwmsUser();
			swmsUser.Email = signUpDto.Email;
			swmsUser.UserName = signUpDto.UserName;
			return await _userManager.CreateAsync(swmsUser, signUpDto.Password);
		}


		async Task<object> ISwmsAuthorizer.Authenticate(ControllerBase controllerBase, LoginDto loginDto)
		{
			SignInResult signInResult = await SignInAsync(loginDto);
			if (!signInResult.Succeeded) return controllerBase.BadRequest(signInResult);

			SwmsUser appUser = await _userManager.FindByNameAsync(loginDto.UserName);
			return _jwtGenerator.GenerateJwtToken(appUser);
		}


		private async Task<SignInResult> SignInAsync(LoginDto loginDto)
		{
			string userName = loginDto.UserName;
			string password = loginDto.Password;
			return await _signInManager.PasswordSignInAsync(userName, password, false, false);
		}
	}
}