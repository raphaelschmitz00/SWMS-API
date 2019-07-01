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
		public IActionResult Authenticate([FromBody] User userParam)
		{
			string userParamUsername = userParam.Username;
			string userParamPassword = userParam.Password;

			User user = _swmsContext.Users.SingleOrDefault(
				x => x.Username == userParamUsername && _passwordHasher.Verify(userParamPassword, x.Password));
			if (user == null) return BadRequest(new {message = "Username or password is incorrect"});

			user.Token = _jwtFactory.CreateToken(user.Id);
			user.Password = null;
			return Ok(user);
		}


		[HttpGet]
		public async Task<ActionResult<IEnumerable<User>>> Index()
		{
			List<User> users = await _swmsContext.Users.ToListAsync();
			return users.Select(x =>
			{
				x.Password = null;
				return x;
			}).ToList();
		}
		
		

		[HttpGet("{id}")]
		public async Task<ActionResult<User>> Get(long id)
		{
			User user = await _swmsContext.Users.FindAsync(id);
			if (user == null) return NotFound();
			return user;
		}


		[HttpPost]
		public async Task<ActionResult<User>> Create(User client)
		{
			_swmsContext.Users.Add(client);
			await _swmsContext.SaveChangesAsync();
			return CreatedAtAction(nameof(Get), new {id = client.Id}, client);
		}


		[HttpPut]
		public async Task<IActionResult> Put(User user)
		{
			_swmsContext.Entry(user).State = EntityState.Modified;
			await _swmsContext.SaveChangesAsync();
			return NoContent();
		}


		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(long id)
		{
			User user = await _swmsContext.Users.FindAsync(id);
			if (user == null) return NotFound();

			_swmsContext.Users.Remove(user);
			await _swmsContext.SaveChangesAsync();

			return NoContent();
		}
	}
}