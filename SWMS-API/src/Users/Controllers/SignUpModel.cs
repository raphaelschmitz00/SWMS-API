using System.ComponentModel.DataAnnotations;


namespace SwmsApi.Users.Controllers
{
	public class SignUpModel
	{
		[Required]
		public string Email { get; set; }

		[Required]
		public string UserName { get; set; }

		[Required]
		public string Password { get; set; }
	}
}