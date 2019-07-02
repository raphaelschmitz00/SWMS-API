using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace SwmsApi.Users.Controllers
{
	[Authorize]
	[ApiController]
	[Route("[controller]")]
	public class UsersController : ControllerBase
	{
		private readonly SwmsContext _swmsContext;
		private readonly IPasswordHasher _passwordHasher;
		private readonly IJwtFactory _jwtFactory;


		public UsersController(SwmsContext swmsContext, IPasswordHasher passwordHasher, IJwtFactory jwtFactory)
		{
			_swmsContext = swmsContext;
			_jwtFactory = jwtFactory;
			_passwordHasher = passwordHasher;
		}


		[AllowAnonymous]
		[HttpPost("authenticate")]
		public IActionResult Authenticate([FromBody] SwmsUser swmsUserParam)
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
			if (swmsUser == null) return NotFound();
			return swmsUser;
		}


		[HttpPost]
		public async Task<ActionResult<SwmsUser>> Create(SwmsUser client)
		{
			_swmsContext.Users.Add(client);
			await _swmsContext.SaveChangesAsync();
			return CreatedAtAction(nameof(Get), new {id = client.Id}, client);
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