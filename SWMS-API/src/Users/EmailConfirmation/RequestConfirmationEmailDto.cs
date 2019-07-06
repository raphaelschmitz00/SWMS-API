using System.ComponentModel.DataAnnotations;


namespace SwmsApi.Users.EmailConfirmation
{
	public class RequestConfirmationEmailDto
	{
		[Required]
		public string Email { get; set; }
	}
}