using Microsoft.AspNetCore.Identity;


namespace SwmsApi.Users
{
	public class SwmsUser : IdentityUser<long>
	{
		public string Token { get; set; }
	}
}