using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFramework.Study.Domain.Common;
using EntityFramework.Study.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using static System.Linq.Expressions.Expression;

namespace EntityFramework.Study.Domain
{
    public class ApplicationDb : DbContext
    {
        public ApplicationDb(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                var entityClrType = entity.ClrType;
                if (!entityClrType.GetInterfaces().Contains(typeof(IStatus)))
                    continue;

                var parameter = Parameter(entityClrType);
                var status = PropertyOrField(parameter, nameof(IStatus.Status));
                entity.QueryFilter = Lambda(NotEqual(status, Constant(Status.Deleted)), parameter);;
            }
        }

        public override int SaveChanges()
        {
            UpdateStatus();
            UpdateDateTimes();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            UpdateStatus();
            UpdateDateTimes();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateDateTimes()
        {
            foreach (var entry in ChangeTracker.Entries<EntityBase>())
            {
                var entity = entry.Entity;
                var now = DateTime.UtcNow;
                switch (entry.State)
                {
                    case EntityState.Added:
                        entity.CreatedAt = now;
                        entity.UpdatedAt = now;
                        break;
                    case EntityState.Modified:
                        entity.UpdatedAt = now;
                        break;
                }
            }
        }

        private void UpdateStatus()
        {
            foreach (var entry in ChangeTracker.Entries<IStatus>())
            {
                var entity = entry.Entity;
                switch (entry.State)
                {
                    case EntityState.Added:
                        entity.Status = Status.Created;
                        break;
                    case EntityState.Modified:
                        entity.Status = Status.Updated;
                        break;
                    case EntityState.Deleted:
                        entity.Status = Status.Deleted;
                        entry.State = EntityState.Modified;
                        break;
                }
            }
        }
    }
}