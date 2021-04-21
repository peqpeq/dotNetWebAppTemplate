using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Contracts.DAL.Base;
using Contracts.Domain;
using Domain.App.Entity;
using Domain.App.Identity;
using Domain.Base;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL.App.EF
{
    public class AppDbContext: IdentityDbContext<AppUser, AppRole, Guid>, IBaseEntityTracker
    {
        public DbSet<TestEntity> TestEntities { get; set; } = default!;
        

        private readonly Dictionary<IDomainEntityId<Guid>, IDomainEntityId<Guid>> _entityTracker =
            new Dictionary<IDomainEntityId<Guid>, IDomainEntityId<Guid>>();

        public AppDbContext(DbContextOptions options)
            : base(options)
        {
        }
         
         protected override void OnModelCreating(ModelBuilder builder)
         {
             base.OnModelCreating(builder);

             // disable cascade delete
             foreach (var relationship in builder.Model
                 .GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
             {
                 relationship.DeleteBehavior = DeleteBehavior.Restrict;
             }


         }

         // Add entity to tracked 
         public void AddToEntityTracker(IDomainEntityId<Guid> internalEntity, IDomainEntityId<Guid> externalEntity)
         {
             _entityTracker.Add(internalEntity, externalEntity);
         }
         
         private void UpdateTrackedEntities()
         {
             foreach (var (key, value) in _entityTracker)
             {
                 value.Id = key.Id;
             }
         }
         
         private void SaveChangesMetadataUpdate()
         {
             // update the state of ef tracked objects
             ChangeTracker.DetectChanges();

             var markedAsAdded = ChangeTracker.Entries().Where(x => x.State == EntityState.Added);
             foreach (var entityEntry in markedAsAdded)
             {
                 if (!(entityEntry.Entity is IDomainEntityMetadata entityWithMetaData)) continue;
                 entityWithMetaData.CreatedAt = DateTime.Now;
                 entityWithMetaData.CreatedBy = "_userNameProvider.CurrentUserName";

                 // if (entityEntry.Entity is IDomainEntityUser<AppUser> entryEntityWithUser)
                 // {
                 //     entryEntityWithUser.AppUserId = _userNameProvider
                 // }
             }
         }

         public override int SaveChanges()
         {
             SaveChangesMetadataUpdate();
             var result = base.SaveChanges();
             UpdateTrackedEntities();
             return result;
         }
         
         public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
         {
             SaveChangesMetadataUpdate();
             var result = base.SaveChangesAsync(cancellationToken);
             UpdateTrackedEntities();
             return result;
         }
         
    }

}
