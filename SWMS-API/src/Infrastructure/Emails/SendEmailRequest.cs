namespace SwmsApi.Infrastructure.Emails
{
	public class SendEmailRequest
	{
		public SendEmailRequest(string email, string subject, string message)
		{
			Email = email;
			Subject = subject;
			Message = message;
		}


		public string Email { get; }
		public string Subject { get; }
		public string Message { get; }
	}
}