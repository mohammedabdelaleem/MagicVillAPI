using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}

	public DbSet<ApplicationUser> ApplicationUsers { get; set; }
	public DbSet<LocalUser> Users { get; set; }
	public DbSet<Villa> Villas { get; set; }
	public DbSet<VillaNumber> VillaNumbers { get; set; }


	protected override void OnModelCreating(ModelBuilder builder)
	{




		base.OnModelCreating(builder);
	}

}
