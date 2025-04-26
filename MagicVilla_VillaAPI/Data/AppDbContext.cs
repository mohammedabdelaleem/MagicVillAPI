using MagicVilla_VillaAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace MagicVilla_VillaAPI.Data;

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
	{
	}

	public DbSet<Villa> Villas { get; set; }
	public DbSet<VillaNumber> VillaNumbers { get; set; }




}
