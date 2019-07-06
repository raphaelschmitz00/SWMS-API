using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SwmsApi.Users.Authorization;


namespace SwmsApi.Users.Controllers
{
	public interface ISwmsAuthorizer
	{
		Task<ActionResult<SwmsUser>> SignUp(ControllerBase controllerBase, SignUpDto signUpDto);
		Task<object> Authenticate(ControllerBase controllerBase, LoginDto loginDto);
	}
}