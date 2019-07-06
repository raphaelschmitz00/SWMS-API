using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


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
		private readonly IUserConfirmationEmailSender _userConfirmationEmailSender;


		public UsersController(SwmsContext swmsContext, IPasswordHasher passwordHasher, IJwtFactory jwtFactory,
			UserManager<SwmsUser> userManager, IUserConfirmationEmailSender userConfirmationEmailSender)
		{
			_swmsContext = swmsContext;
			_passwordHasher = passwordHasher;
			_jwtFactory = jwtFactory;
			_userManager = userManager;
			_userConfirmationEmailSender = userConfirmationEmailSender;
		}


		[HttpPost("sign-up")]
		public async Task<ActionResult<SwmsUser>> SignUp(SignUpModel signUpModel)
		{
			SwmsUser swmsUser = new SwmsUser();
			swmsUser.Email = signUpModel.Email;
			swmsUser.UserName = signUpModel.UserName;
			IdentityResult identityResult = await _userManager.CreateAsync(swmsUser, signUpModel.Password);
			if (!identityResult.Succeeded) return BadRequest(identityResult.Errors);
			return Ok(swmsUser);
		}


		[HttpPost("request-confirmation-email")]
		public async Task<ActionResult<SwmsUser>> RequestConfirmationEmail([FromBody]string email)
		{
			SwmsUser swmsUser = await _userManager.FindByEmailAsync(email);
			if (swmsUser == null) return BadRequest("Can't find user!");

			await _userConfirmationEmailSender.Send(Url, Request, swmsUser);
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


		[HttpGet("{id}")]
		public async Task<ActionResult<SwmsUser>> Get(long id)
		{
			SwmsUser swmsUser = await _swmsContext.Users.FindAsync(id);
			if (swmsUser == null) return BadRequest();
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