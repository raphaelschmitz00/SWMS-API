using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SwmsApi.Users.Authorization;
using SwmsApi.Users.EmailConfirmation;


namespace SwmsApi.Users.Controllers
{
	public interface IUserEmailConfirmer
	{
		Task<ActionResult<SwmsUser>> RequestConfirmationEmail(ControllerBase controllerBase,
			RequestConfirmationEmailDto requestConfirmationEmailDto);


		Task<ActionResult<SwmsUser>> ConfirmEmail(ControllerBase controllerBase,
			ConfirmEmailDto confirmEmailDto);
	}
}