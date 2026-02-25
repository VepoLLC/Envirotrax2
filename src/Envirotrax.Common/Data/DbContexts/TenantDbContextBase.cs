
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;
using Envirotrax.Common.Data.Services.Definitions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;

namespace Envirotrax.Common.Data.DbContexts
{
    public class TenantDbContextBase<TTenant> : TenantDbContextBase<TTenant, AspNetUserBase>
        where TTenant : TenantBase
    {
        public TenantDbContextBase(
            DbContextOptions options,
            ILogger<TenantDbContextBase<TTenant, AspNetUserBase>> logger,
            ITenantProvidersService tenantProvider)
            : base(options, logger, tenantProvider)
        {

        }
    }

    public class TenantDbContextBase<TTenant, TUser> : DbContext
        where TTenant : TenantBase
        where TUser : AspNetUserBase
    {
        private readonly ILogger<TenantDbContextBase<TTenant, TUser>> _logger;
        private readonly ITenantProvidersService _tenantProvider;

        public bool SkipSaveSecurityProperties { get; set; }

        public DbSet<TTenant> WaterSuppliers { get; set; }
        public DbSet<TUser> AspNetUsers { get; set; }

        public TenantDbContextBase(
            DbContextOptions options,
            ILogger<TenantDbContextBase<TTenant, TUser>> logger,
            ITenantProvidersService tenantProvider)
            : base(options)
        {
            _logger = logger;
            _tenantProvider = tenantProvider;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                SetPrimaryKeys(modelBuilder, entity);
                SetForeignKeys(modelBuilder, entity);
                SetIndexes(modelBuilder, entity);
                SetupGlobalFiltering(modelBuilder, entity);

                foreach (var relationship in entity.GetForeignKeys())
                {
                    relationship.DeleteBehavior = DeleteBehavior.Restrict;
                }

                if (entity.ClrType.GetCustomAttribute<ExcludedModelAttribute>() != null)
                {
                    modelBuilder.Entity(entity.ClrType).Metadata.SetIsTableExcludedFromMigrations(true);
                }

                var waterSupplierProperty = entity.ClrType.GetProperty(nameof(TenantModel<>.WaterSupplier));
                if (waterSupplierProperty != null && typeof(TenantBase).IsAssignableFrom(waterSupplierProperty.PropertyType))
                {
                    modelBuilder.Entity(entity.ClrType)
                        .HasOne(waterSupplierProperty.PropertyType, nameof(TenantModel<>.WaterSupplier))
                        .WithMany()
                        .HasForeignKey(nameof(ITenantModel.WaterSupplierId));
                }
            }
        }

        // Currently, EF has a bug, so we use a workaround for it.
        // https://github.com/dotnet/efcore/issues/10257
        protected LambdaExpression ConvertFilterExpression<TInterface>(Expression<Func<TInterface, bool>> filterExpression, Type entityType)
        {
            var newParam = Expression.Parameter(entityType);
            var newBody = ReplacingExpressionVisitor.Replace(filterExpression.Parameters.Single(), newParam, filterExpression.Body);

            return Expression.Lambda(newBody, newParam);
        }

        private Expression<Func<ITenantModel, bool>> GetTenantFilterExpression()
        {
            return t => t.WaterSupplierId == _tenantProvider.WaterSupplierId;
        }

        protected virtual void SetupGlobalFiltering(ModelBuilder builder, IMutableEntityType entity)
        {
            if (typeof(ITenantModel).IsAssignableFrom(entity.ClrType))
            {
                Expression<Func<ITenantModel, bool>> expression = GetTenantFilterExpression();
                LambdaExpression lambdaExpression = ConvertFilterExpression(expression, entity.ClrType);

                builder.Entity(entity.ClrType).HasQueryFilter(lambdaExpression);
            }
        }

        private void EnforceReadOnlyTables()
        {
            var errorEntries = ChangeTracker
                .Entries()
                .Where(e => e.Metadata.ClrType.GetCustomAttribute<ReadOnlyModelAttribute>() != null && e.State != EntityState.Unchanged);

            if (errorEntries.Any())
            {
                foreach (var entry in errorEntries)
                {
                    entry.State = EntityState.Unchanged;
                }

                var errors = errorEntries
                    .Select(e => e.Metadata.Name)
                    .Distinct()
                    .ToList();

                _logger.LogError(new InvalidOperationException($"Attempted to save read-only entities {string.Join(",", errors)}"), "Error saving a readonly model.");
            }
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

        private void SetForeignKeys(ModelBuilder builder, IMutableEntityType entity)
        {
            var navigationalProperties = entity
                .ClrType
                .GetProperties()
                .Where(p => typeof(ITenantModel).IsAssignableFrom(p.PropertyType) && p.GetCustomAttribute<NotMappedAttribute>() == null);

            foreach (var property in navigationalProperties)
            {
                var foreignKey = entity.ClrType.GetProperty(property.Name + "Id");

                if (foreignKey != null)
                {
                    builder.Entity(entity.ClrType)
                      .HasOne(property.PropertyType, property.Name)
                      .WithMany()
                      .HasForeignKey(nameof(ITenantModel.WaterSupplierId), foreignKey.Name);
                }
            }
        }

        private void SetIndexes(ModelBuilder builder, IMutableEntityType entity)
        {
            var indexes = entity
                .ClrType
                .GetCustomAttributes<AppIndexAttribute>();

            foreach (var index in indexes)
            {
                var list = new List<string>(index.PropertyNames);

                if (typeof(ITenantModel).IsAssignableFrom(entity.ClrType))
                {
                    if (!index.PropertyNames.Contains(nameof(ITenantModel.WaterSupplierId)))
                    {
                        list.Insert(0, nameof(ITenantModel.WaterSupplierId));
                    }
                }

                builder.Entity(entity.ClrType)
                        .HasIndex(list.ToArray())
                        .IsUnique(index.IsUnique);
            }
        }

        private void CheckDuplicateException(Exception ex)
        {
            if (ex is DbUpdateException dbUpdateEx)
            {
                if (dbUpdateEx.InnerException is SqlException sqlEx)
                {
                    if (sqlEx.Number == 2627)
                    {
                        // In case if we are adding the same record again.
                        throw new DuplicateRecordException("The record with provided key already exists", ex);
                    }

                    if (sqlEx.Number == 2601)
                    {
                        // In case if we have a unique index error.
                        throw new DuplicateRecordException(ex);
                    }
                }
            }
        }

        protected virtual int VerifySaveTenantId(int tenantId)
        {
            // We may need to later skip this.
            return _tenantProvider.WaterSupplierId;
        }

        public void SetTenantId(object entity)
        {
            if (!SkipSaveSecurityProperties)
            {
                if (entity is ITenantModel tenantModel)
                {
                    tenantModel.WaterSupplierId = VerifySaveTenantId(_tenantProvider.WaterSupplierId);
                }
            }
        }

        /// <summary>
        /// This will make sure that the tenant ID is set on every DML query so we don't have to do that every time.
        /// </summary>
        public void SetTenantId()
        {
            if (!SkipSaveSecurityProperties)
            {
                var tenantId = _tenantProvider.WaterSupplierId;

                foreach (var entry in ChangeTracker.Entries<ITenantModel>())
                {
                    var tenantIdProperty = entry.Property(e => e.WaterSupplierId);
                    tenantIdProperty.CurrentValue = VerifySaveTenantId(tenantIdProperty.CurrentValue);
                }
            }
        }

        protected virtual void SetSecurityProperties(object entity)
        {
            SetTenantId(entity);
        }

        protected virtual void SetSecurityProperties()
        {
            SetTenantId();
        }

        private bool HasGenericInterface(Type type, Type genericInterfaceType)
        {
            return type
                .GetInterfaces()
                .Any(x =>
                    x.IsGenericType &&
                    x.GetGenericTypeDefinition() == genericInterfaceType)
                ;
        }

        private void AuditEntities()
        {
            if (_tenantProvider.UserId > 0)
            {
                foreach (var entry in ChangeTracker.Entries())
                {
                    var type = entry.Entity.GetType();

                    if (HasGenericInterface(type, typeof(ICreateAuditableModel<>)))
                    {
                        if (entry.State == EntityState.Added)
                        {
                            entry.Property(nameof(ICreateAuditableModel<AspNetUserBase>.CreatedById)).CurrentValue = _tenantProvider.UserId;
                            entry.Property(nameof(ICreateAuditableModel<AspNetUserBase>.CreatedTime)).CurrentValue = DateTime.UtcNow;
                        }
                        else
                        {
                            entry.Property(nameof(ICreateAuditableModel<AspNetUserBase>.CreatedById)).IsModified = false;
                            entry.Property(nameof(ICreateAuditableModel<AspNetUserBase>.CreatedTime)).IsModified = false;
                        }
                    }

                    if (HasGenericInterface(type, typeof(IUpdateAuditableModel<>)))
                    {
                        if (entry.State == EntityState.Modified)
                        {
                            entry.Property(nameof(IUpdateAuditableModel<AspNetUserBase>.UpdatedById)).CurrentValue = _tenantProvider.UserId;
                            entry.Property(nameof(IUpdateAuditableModel<AspNetUserBase>.UpdatedTime)).CurrentValue = DateTime.UtcNow;
                        }
                        else
                        {
                            entry.Property(nameof(IUpdateAuditableModel<AspNetUserBase>.UpdatedById)).IsModified = false;
                            entry.Property(nameof(IUpdateAuditableModel<AspNetUserBase>.UpdatedTime)).IsModified = false;
                        }
                    }

                    if (HasGenericInterface(type, typeof(IDeleteAutitableModel<>)))
                    {
                        if (entry.State == EntityState.Deleted)
                        {
                            entry.State = EntityState.Modified;

                            entry.Property(nameof(IDeleteAutitableModel<AspNetUserBase>.DeletedById)).CurrentValue = _tenantProvider.UserId;
                            entry.Property(nameof(IDeleteAutitableModel<AspNetUserBase>.DeletedTime)).CurrentValue = DateTime.UtcNow;
                        }
                        else
                        {
                            entry.Property(nameof(IDeleteAutitableModel<AspNetUserBase>.DeletedById)).IsModified = false;
                            entry.Property(nameof(IDeleteAutitableModel<AspNetUserBase>.DeletedTime)).IsModified = false;
                        }
                    }
                }
            }

        }

        public override int SaveChanges()
        {
            EnforceReadOnlyTables();
            SetSecurityProperties();
            AuditEntities();

            try
            {
                return base.SaveChanges();
            }
            catch (Exception ex)
            {
                CheckDuplicateException(ex);
                throw;
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            EnforceReadOnlyTables();
            SetSecurityProperties();
            AuditEntities();

            try
            {
                return await base.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                CheckDuplicateException(ex);
                throw;
            }
        }

        public override EntityEntry Attach(object entity)
        {
            SetSecurityProperties(entity);
            return base.Attach(entity);
        }

        public override EntityEntry<TEntity> Attach<TEntity>(TEntity entity)
        {
            SetSecurityProperties(entity);
            return base.Attach(entity);
        }

        public override void AttachRange(params object[] entities)
        {
            foreach (var entity in entities)
            {
                SetSecurityProperties(entity);
            }

            base.AttachRange(entities);
        }

        public override void AttachRange(IEnumerable<object> entities)
        {
            foreach (var entity in entities)
            {
                SetSecurityProperties(entity);
            }

            base.AttachRange(entities);
        }

        public override EntityEntry Entry(object entity)
        {
            SetSecurityProperties(entity);
            return base.Entry(entity);
        }

        public override EntityEntry<TEntity> Entry<TEntity>(TEntity entity)
        {
            SetSecurityProperties(entity);
            return base.Entry(entity);
        }
    }
}