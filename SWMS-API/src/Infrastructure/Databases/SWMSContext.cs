using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SwmsApi.Clients;
using SwmsApi.Users;


namespace SwmsApi.Infrastructure.Databases
{
	public class SwmsContext : IdentityDbContext<SwmsUser, IdentityRole<long>, long>
	{
		public SwmsContext(DbContextOptions<SwmsContext> options) : base(options)
		{
		}


		public DbSet<Client> Clients { get; set; }
	}
}