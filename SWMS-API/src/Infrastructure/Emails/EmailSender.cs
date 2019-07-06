using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;


namespace SwmsApi.Infrastructure.Emails
{
	public class EmailSender : IEmailSender
	{
		private readonly EmailSettings _emailSettings;


		public EmailSender(IOptions<EmailSettings> emailSettings)
		{
			_emailSettings = emailSettings.Value;
		}


		public async Task SendEmailAsync(SendEmailRequest sendEmailRequest)
		{
			MailboxAddress from = new MailboxAddress(_emailSettings.SenderName, _emailSettings.Sender);
			MailboxAddress to = new MailboxAddress(sendEmailRequest.Email);
			string subject = sendEmailRequest.Subject;
			TextPart textPart = new TextPart("html") {Text = sendEmailRequest.Message};
			MimeMessage mimeMessage = CreateMimeMessage(@from, to, subject, textPart);
			await Send(mimeMessage);
		}


		private static MimeMessage CreateMimeMessage(MailboxAddress from, MailboxAddress to, string subject,
			TextPart textPart)
		{
			MimeMessage mimeMessage = new MimeMessage();
			mimeMessage.From.Add(from);
			mimeMessage.To.Add(to);
			mimeMessage.Subject = subject;
			mimeMessage.Body = textPart;
			return mimeMessage;
		}


		private async Task Send(MimeMessage mimeMessage)
		{
			using (SmtpClient client = new SmtpClient())
			{
				await client.ConnectAsync(_emailSettings.MailServer, _emailSettings.MailPort);
				await client.AuthenticateAsync(_emailSettings.Sender, _emailSettings.Password);
				await client.SendAsync(mimeMessage);
				await client.DisconnectAsync(true);
			}
		}
	}
}