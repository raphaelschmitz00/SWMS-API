using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace SwmsApi.Users.Controllers
{
	public interface IUserConfirmationEmailSender
	{
		Task Send(IUrlHelper urlHelper, HttpRequest httpRequest, SwmsUser swmsUser);
	}
}