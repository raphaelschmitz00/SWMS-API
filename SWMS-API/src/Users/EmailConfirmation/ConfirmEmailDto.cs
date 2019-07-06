namespace SwmsApi.Users.EmailConfirmation
{
	public class ConfirmEmailDto
	{
		public long UserId { get; set; }
		public string Token { get; set; }
	}
}