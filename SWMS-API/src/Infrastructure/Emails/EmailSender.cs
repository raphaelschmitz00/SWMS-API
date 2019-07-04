using System;
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


		public async Task SendEmailAsync(string email, string subject, string message)
		{
			try
			{
				MimeMessage mimeMessage = new MimeMessage();
				mimeMessage.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.Sender));
				mimeMessage.To.Add(new MailboxAddress(email));
				mimeMessage.Subject = subject;
				mimeMessage.Body = new TextPart("html")
				{
					Text = message
				};

				using (SmtpClient client = new SmtpClient())
				{
					// For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
					//client.ServerCertificateValidationCallback = (s, c, h, e) => true;

					await client.ConnectAsync(_emailSettings.MailServer, _emailSettings.MailPort);
					await client.AuthenticateAsync(_emailSettings.Sender, _emailSettings.Password);
					await client.SendAsync(mimeMessage);
					await client.DisconnectAsync(true);
				}
			}
			catch (Exception ex)
			{
				// TODO: handle exception
				throw new InvalidOperationException(ex.Message);
			}
		}
	}
}