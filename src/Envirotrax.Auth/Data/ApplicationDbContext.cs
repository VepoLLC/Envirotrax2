using System.Reflection;
using Envirotrax.Auth.Data.Models;
using Envirotrax.Common.Data.Attributes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

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

    private void SetPrimaryKeys(ModelBuilder builder, IMutableEntityType entity)
    {
        var primaryKeyAttributes = entity
            .ClrType
            .GetProperties()
            .Where(p => p.GetCustomAttribute<AppPrimaryKeyAttribute>() != null)
            .Select(p => new
            {
                Property = p,
                Attribute = p.GetCustomAttribute<AppPrimaryKeyAttribute>()
            })
            .OrderBy(p => p.Attribute!.CompositeKeyOrder);

        if (primaryKeyAttributes.Any())
        {
            var keyNames = primaryKeyAttributes
                .Select(a => a.Property.Name)
                .ToArray();

            builder.Entity(entity.ClrType).HasKey(keyNames);

            foreach (var key in primaryKeyAttributes)
            {
                var property = builder
                    .Entity(entity.ClrType)
                    .Property(key.Property.Name);

                if (key.Attribute!.ValueGeneratedOnAdd)
                {
                    property.ValueGeneratedOnAdd();
                }
                else
                {
                    property.ValueGeneratedNever();
                }
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        foreach (var entity in builder.Model.GetEntityTypes())
        {
            SetPrimaryKeys(builder, entity);

            if (entity.ClrType.GetCustomAttribute<ExcludedModelAttribute>() != null)
            {
                builder.Entity(entity.ClrType).Metadata.SetIsTableExcludedFromMigrations(true);
            }
        }
    }
}

