using Microsoft.EntityFrameworkCore;
using SwmsApi.Clients;
using SwmsApi.Users;


namespace SwmsApi
{
	public class SwmsContext : DbContext
	{
		public SwmsContext(DbContextOptions<SwmsContext> options) : base(options)
		{
		}


		//public DbSet<User> Users { get; set; }
		public DbSet<Client> Clients { get; set; }
	}
}