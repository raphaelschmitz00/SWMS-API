using System.Threading.Tasks;


namespace SwmsApi.Infrastructure.Emails
{
	public interface IEmailSender
	{
		Task SendEmailAsync(SendEmailRequest sendEmailRequest);
	}
}