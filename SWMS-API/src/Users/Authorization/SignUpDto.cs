using System.ComponentModel.DataAnnotations;


namespace SwmsApi.Users.Authorization
{
	public class SignUpDto
	{
		[Required]
		public string Email { get; set; }

		[Required]
		public string UserName { get; set; }

		[Required]
		public string Password { get; set; }
	}
}