using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace SwmsApi.Users
{
	[Authorize]
	[ApiController]
	[Route("[controller]")]
	public class UsersController : ControllerBase
	{
		private readonly IUserService _userService;


		public UsersController(IUserService userService)
		{
			_userService = userService;
		}


		[AllowAnonymous]
		[HttpPost("authenticate")]
		public IActionResult Authenticate([FromBody] User userParam)
		{
			User user = _userService.Authenticate(userParam.Username, userParam.Password);

			if (user == null)
				return BadRequest(new {message = "Username or password is incorrect"});

			return Ok(user);
		}


		[HttpGet]
		public IActionResult GetAll()
		{
			IEnumerable<User> users = _userService.GetAll();
			return Ok(users);
		}
	}
}