using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SwmsApi.Infrastructure.Emails;


namespace SwmsApi.Users.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class UsersController : ControllerBase
	{
		private readonly SwmsContext _swmsContext;
		private readonly IPasswordHasher _passwordHasher;
		private readonly IJwtFactory _jwtFactory;
		private readonly UserManager<SwmsUser> _userManager;
		private readonly SignInManager<SwmsUser> _signInManager;
		//private readonly ILogger _logger;
		private readonly IEmailSender _emailSender;


		public UsersController(SwmsContext swmsContext, IPasswordHasher passwordHasher, IJwtFactory jwtFactory,
			UserManager<SwmsUser> userManager, SignInManager<SwmsUser> signInManager, //ILogger logger,
			IEmailSender emailSender)
		{
			_swmsContext = swmsContext;
			_passwordHasher = passwordHasher;
			_jwtFactory = jwtFactory;
			_userManager = userManager;
			_signInManager = signInManager;
			//_logger = logger;
			_emailSender = emailSender;
		}



		[HttpPost("create")]
		public async Task<ActionResult<SwmsUser>> Create(SignUpModel signUpModel)
		{
			SwmsUser swmsUser = new SwmsUser();
			swmsUser.Email = signUpModel.Email;
			swmsUser.UserName = signUpModel.UserName;
			IdentityResult identityResult = await _userManager.CreateAsync(swmsUser, signUpModel.Password);

			if (!identityResult.Succeeded) return BadRequest(new {message = "Can't create user!"});
			
			//	_logger.LogInformation("User created a new account with password.");

			string code = await _userManager.GenerateEmailConfirmationTokenAsync(swmsUser);
			string confirmEmailUrl = Url.Action(nameof(ConfirmEmail), new {userId = swmsUser.Id, token = code});

			string encodedUrl = HtmlEncoder.Default.Encode(confirmEmailUrl);
			string message = $"Please confirm your account by <a href='{encodedUrl}'>clicking here</a>.";
			await _emailSender.SendEmailAsync(swmsUser.Email, "Confirm your email", message);
			return Ok();
		}

		
		[HttpPost("confirm-email")]
		public async Task<ActionResult<SwmsUser>> ConfirmEmail(long userId, string token)
		{
			SwmsUser swmsUser = await _userManager.FindByIdAsync(userId.ToString());
			if (swmsUser == null) return BadRequest();
			
			IdentityResult identityResult = await _userManager.ConfirmEmailAsync(swmsUser, token);
			if (identityResult.Succeeded) return Ok();
			
			return BadRequest();
		}


		[HttpPost("createo")]
		public IActionResult Create(SwmsUser swmsUserParam)
		{
			string userParamUsername = swmsUserParam.UserName;
			string userParamPassword = swmsUserParam.PasswordHash;

			SwmsUser swmsUser = _swmsContext.Users.SingleOrDefault(
				x => x.UserName == userParamUsername && _passwordHasher.Verify(userParamPassword, x.PasswordHash));
			if (swmsUser == null) return BadRequest(new {message = "Username or password is incorrect"});

			swmsUser.Token = _jwtFactory.CreateToken(swmsUser.Id);
			swmsUser.PasswordHash = null;
			return Ok(swmsUser);
		}


		[HttpPost("authenticate")]
		public IActionResult Authenticate(SwmsUser swmsUserParam)
		{
			string userParamUsername = swmsUserParam.UserName;
			string userParamPassword = swmsUserParam.PasswordHash;

			SwmsUser swmsUser = _swmsContext.Users.SingleOrDefault(
				x => x.UserName == userParamUsername && _passwordHasher.Verify(userParamPassword, x.PasswordHash));
			if (swmsUser == null) return BadRequest(new {message = "Username or password is incorrect"});

			swmsUser.Token = _jwtFactory.CreateToken(swmsUser.Id);
			swmsUser.PasswordHash = null;
			return Ok(swmsUser);
		}


		[HttpGet]
		public async Task<ActionResult<IEnumerable<SwmsUser>>> Index()
		{
			List<SwmsUser> users = await _swmsContext.Users.ToListAsync();
			return users.Select(x =>
			{
				x.PasswordHash = null;
				return x;
			}).ToList();
		}


		//[Authorize]
		[HttpGet("{id}")]
		public async Task<ActionResult<SwmsUser>> Get(long id)
		{
			SwmsUser swmsUser = await _swmsContext.Users.FindAsync(id);
			if (swmsUser == null) return NotFound();
			return swmsUser;
		}


		[HttpPut]
		public async Task<IActionResult> Put(SwmsUser swmsUser)
		{
			_swmsContext.Entry(swmsUser).State = EntityState.Modified;
			await _swmsContext.SaveChangesAsync();
			return NoContent();
		}


		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(long id)
		{
			SwmsUser swmsUser = await _swmsContext.Users.FindAsync(id);
			if (swmsUser == null) return NotFound();

			_swmsContext.Users.Remove(swmsUser);
			await _swmsContext.SaveChangesAsync();

			return NoContent();
		}
	}
}