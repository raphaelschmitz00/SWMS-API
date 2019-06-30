using System.Collections.Generic;


namespace SwmsApi.Users
{
	public interface IUserService
	{
		User Authenticate(string username, string password);
		IEnumerable<User> GetAll();
	}
}