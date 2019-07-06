using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SwmsApi.Infrastructure.Emails;
using SwmsApi.Users.Controllers;


namespace SwmsApi.Users
{
	public class UserConfirmationEmailSender : IUserConfirmationEmailSender
	{
		private readonly UserManager<SwmsUser> _userManager;
		private readonly IEmailSender _emailSender;


		public UserConfirmationEmailSender(UserManager<SwmsUser> userManager, IEmailSender emailSender)
		{
			_userManager = userManager;
			_emailSender = emailSender;
		}


		public async Task Send(IUrlHelper urlHelper, HttpRequest httpRequest, SwmsUser swmsUser)
		{
			string code = await _userManager.GenerateEmailConfirmationTokenAsync(swmsUser);
			string confirmEmailUrl = urlHelper.Action(nameof(UsersController.ConfirmEmail),
				null,
				new {userId = swmsUser.Id, token = code},
				httpRequest.Scheme
			);
			string encodedUrl = HtmlEncoder.Default.Encode(confirmEmailUrl);
			string message = $"Please confirm your account by <a href='{encodedUrl}'>clicking here</a>.";
			SendEmailRequest sendEmailRequest = new SendEmailRequest(swmsUser.Email, "Confirm your email", message);

			await _emailSender.SendEmailAsync(sendEmailRequest);
		}
	}
}