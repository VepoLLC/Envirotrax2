using Envirotrax.Auth.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.Auth.Data;

public class ApplicationDbContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
{
    public DbSet<WaterSupplier> WaterSuppliers { get; set; }
    public DbSet<WaterSupplierUser> WaterSupplierUsers { get; set; }

    public DbSet<Contractor> Contractors { get; set; }
    public DbSet<ContractorUser> ContractorUsers { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
          : base(options)
    {
    }
}

