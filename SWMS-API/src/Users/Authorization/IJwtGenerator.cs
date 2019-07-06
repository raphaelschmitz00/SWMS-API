namespace SwmsApi.Users.Authorization
{
	public interface IJwtGenerator
	{
		object GenerateJwtToken(SwmsUser user);
	}
}