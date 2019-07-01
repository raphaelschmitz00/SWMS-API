namespace SwmsApi.Users.Controllers
{
	public interface IJwtFactory
	{
		string CreateToken(long userId);
	}
}