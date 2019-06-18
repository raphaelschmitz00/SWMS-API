using Microsoft.EntityFrameworkCore;
using SwmsApi.Clients;


namespace SwmsApi
{
	public class SwmsContext : DbContext
	{
		public SwmsContext(DbContextOptions<SwmsContext> options) : base(options)
		{
		}


		public DbSet<Client> Clients { get; set; }
	}
}