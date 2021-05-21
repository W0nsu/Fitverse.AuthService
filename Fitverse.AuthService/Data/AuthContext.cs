using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data
{
	public class AuthContext : DbContext
	{
		public AuthContext(DbContextOptions<AuthContext> options) : base(options)
		{
		}

		public DbSet<User> Users { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.Entity<User>()
				.HasKey(x => x.UserId);
			
			builder.Entity<User>()
				.Property(x => x.UserId)
				.ValueGeneratedOnAdd();

			builder.Entity<User>()
				.Property(x => x.Username)
				.IsRequired()
				.HasMaxLength(20);

			builder.Entity<User>()
				.Property(x => x.Password)
				.IsRequired();
		}
	}
}