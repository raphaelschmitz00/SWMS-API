using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SwmsApi.Infrastructure.Emails;
using SwmsApi.Users.Controllers;


namespace SwmsApi.Users.EmailConfirmation
{
	public class UserEmailConfirmer : IUserEmailConfirmer
	{
		private readonly UserManager<SwmsUser> _userManager;
		private readonly IEmailSender _emailSender;


		public UserEmailConfirmer(UserManager<SwmsUser> userManager, IEmailSender emailSender)
		{
			_userManager = userManager;
			_emailSender = emailSender;
		}


		async Task<ActionResult<SwmsUser>> IUserEmailConfirmer.ConfirmEmail(ControllerBase controllerBase,
			ConfirmEmailDto confirmEmailDto)
		{
			SwmsUser swmsUser = await _userManager.FindByIdAsync(confirmEmailDto.UserId.ToString());
			if (swmsUser == null) return controllerBase.NotFound();

			IdentityResult identityResult = await _userManager.ConfirmEmailAsync(swmsUser, confirmEmailDto.Token);
			if (identityResult.Succeeded) return controllerBase.Ok();

			return controllerBase.BadRequest();
		}


		async Task<ActionResult<SwmsUser>> IUserEmailConfirmer.RequestConfirmationEmail(ControllerBase controllerBase,
			RequestConfirmationEmailDto requestConfirmationEmailDto)
		{
			SwmsUser swmsUser = await _userManager.FindByEmailAsync(requestConfirmationEmailDto.Email);

			if (swmsUser == null) return controllerBase.NotFound();
			string token = await _userManager.GenerateEmailConfirmationTokenAsync(swmsUser);

			string confirmEmailUrl = CreateConfirmationUrl(controllerBase, swmsUser.Id, token);
			SendEmailRequest sendEmailRequest = CreateConfirmationEmailRequest(confirmEmailUrl, swmsUser);
			await _emailSender.SendEmailAsync(sendEmailRequest);

			return controllerBase.Ok();
		}


		private static SendEmailRequest CreateConfirmationEmailRequest(string confirmEmailUrl, SwmsUser swmsUser)
		{
			string encodedUrl = HtmlEncoder.Default.Encode(confirmEmailUrl);
			string message = $"Please confirm your account by <a href='{encodedUrl}'>clicking here</a>.";
			SendEmailRequest sendEmailRequest = new SendEmailRequest(swmsUser.Email, "Confirm your email", message);
			return sendEmailRequest;
		}


		private static string CreateConfirmationUrl(ControllerBase controllerBase, long userId, string token)
		{
			string action = nameof(UsersController.ConfirmEmail);
			ConfirmEmailDto confirmEmailDto = new ConfirmEmailDto {UserId = userId, Token = token};
			string protocol = controllerBase.Request.Scheme;
			return controllerBase.Url.Action(action, null, confirmEmailDto, protocol);
		}
	}
}