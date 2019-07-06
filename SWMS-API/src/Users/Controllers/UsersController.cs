using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SwmsApi.Users.Authorization;
using SwmsApi.Users.EmailConfirmation;


namespace SwmsApi.Users.Controllers
{
	[Authorize]
	[ApiController]
	[Route("[controller]")]
	public class UsersController : ControllerBase
	{
		private readonly UserManager<SwmsUser> _userManager;
		private readonly IUserEmailConfirmer _userEmailConfirmer;
		private readonly ISwmsAuthorizer _swmsAuthorizer;


		public UsersController(UserManager<SwmsUser> userManager, IUserEmailConfirmer userEmailConfirmer,
			ISwmsAuthorizer swmsAuthorizer)
		{
			_userManager = userManager;
			_userEmailConfirmer = userEmailConfirmer;
			_swmsAuthorizer = swmsAuthorizer;
		}


		[AllowAnonymous]
		[HttpPost("sign-up")]
		public async Task<ActionResult<SwmsUser>> SignUp(SignUpDto signUpDto)
		{
			return await _swmsAuthorizer.SignUp(this, signUpDto);
		}


		[AllowAnonymous]
		[HttpPost("sign-in")]
		public async Task<object> Authenticate(LoginDto loginDto)
		{
			return await _swmsAuthorizer.Authenticate(this, loginDto);
		}


		[HttpPost("request-confirmation-email")]
		public async Task<ActionResult<SwmsUser>> RequestConfirmationEmail(
			RequestConfirmationEmailDto requestConfirmationEmailDto)
		{
			return await _userEmailConfirmer.RequestConfirmationEmail(this, requestConfirmationEmailDto);
		}


		[HttpPost("confirm-email")]
		public async Task<ActionResult<SwmsUser>> ConfirmEmail(ConfirmEmailDto confirmEmailDto)
		{
			return await _userEmailConfirmer.ConfirmEmail(this, confirmEmailDto);
		}


		[HttpGet]
		public async Task<ActionResult<IEnumerable<SwmsUser>>> Index()
		{
			List<SwmsUser> users = await _userManager.Users.ToListAsync();
			return users.Select(x =>
			{
				x.PasswordHash = null;
				return x;
			}).ToList();
		}


		[HttpGet("get/{id}")]
		public async Task<ActionResult<SwmsUser>> Get(long id)
		{
			SwmsUser swmsUser = await _userManager.FindByIdAsync(id.ToString());
			if (swmsUser == null) return NotFound();
			return swmsUser;
		}


		[HttpPut("update")]
		public async Task<IActionResult> Put(SwmsUser swmsUser)
		{
			await _userManager.UpdateAsync(swmsUser);
			return NoContent();
		}


		[HttpDelete("delete/{id}")]
		public async Task<IActionResult> Delete(long id)
		{
			SwmsUser swmsUser = await _userManager.FindByIdAsync(id.ToString());
			if (swmsUser == null) return NotFound();

			await _userManager.DeleteAsync(swmsUser);
			return NoContent();
		}
	}
}