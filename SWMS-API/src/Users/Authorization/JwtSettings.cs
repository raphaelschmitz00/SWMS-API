namespace SwmsApi.Users.Authorization
{
	public class JwtSettings
	{
		public string Secret { get; set; }
		public string Issuer { get; set; }
		public int ExpireDays { get; set; }
	}
}