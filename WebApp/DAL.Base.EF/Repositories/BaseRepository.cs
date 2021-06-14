using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts.DAL.Base;
using Contracts.DAL.Base.Mappers;
using Contracts.DAL.Base.Repositories;
using Contracts.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DAL.Base.EF.Repositories
{
public class BaseRepository<TDbContext, TUser, TDomainEntity, TDALEntity> :
        BaseRepository<Guid, TDbContext, TUser, TDomainEntity, TDALEntity>,
        IBaseRepository<TDALEntity>
        where TDALEntity : class, IDomainEntityId<Guid>, new()
        where TDomainEntity : class, IDomainEntityId<Guid>, new()
        where TUser : IdentityUser<Guid>
        where TDbContext : DbContext, IBaseEntityTracker

    {
        public BaseRepository(TDbContext repoDbContext, IBaseDALMapper<TDomainEntity, TDALEntity> mapper) : base(
            repoDbContext, mapper)
        {
        }
    }

    public class BaseRepository<TKey, TDbContext, TUser, TDomainEntity, TDALEntity> :
        IBaseRepository<TKey, TDALEntity>
        where TDALEntity : class, IDomainEntityId<TKey>, new()
        where TDomainEntity : class, IDomainEntityId<TKey>, new()
        where TUser : IdentityUser<TKey>
        where TDbContext : DbContext, IBaseEntityTracker<TKey>
        where TKey : struct, IEquatable<TKey>


    {
        
        private TDbContext RepoDbContext { get; set; }
        protected DbSet<TDomainEntity> RepoDbSet { get; set; }
        
        protected readonly IBaseDALMapper<TDomainEntity, TDALEntity> Mapper;

        protected BaseRepository(TDbContext repoDbContext, IBaseDALMapper<TDomainEntity, TDALEntity> mapper)
        {
            RepoDbContext = repoDbContext;
            Mapper = mapper;
            RepoDbSet = RepoDbContext.Set<TDomainEntity>();
            if (RepoDbSet == null)
            {
                throw new ArgumentNullException(typeof(TDALEntity).Name + "was not found as DBSet");
            }
        }
        
        
        /// <summary>
        /// Prepares query
        /// </summary>
        /// <param name="userId">by userId if needed, default null </param>
        /// <param name="noTracking">default true</param>
        /// <returns>Query</returns>
        protected IQueryable<TDomainEntity> PrepareQuery(object? userId = null, bool noTracking = true)
        {
            var query = RepoDbSet.AsQueryable();
            // Shall we disable entity tracking
            if (noTracking)
            {
                query = query.AsNoTracking();
            }

            // userId != null and is this entity implementing IDomainEntityUser
            if (userId != null && typeof(IDomainEntityUser<TKey, TUser>).IsAssignableFrom(typeof(TDomainEntity)))
            {
                // accessing TDomainEntity.AppUserId via shadow property access
                query = query.Where(e =>
                    Microsoft.EntityFrameworkCore.EF.Property<TKey>(e, nameof(IDomainEntityUser<TKey, TUser>.AppUserId))
                        .Equals((TKey) userId));
            }

            return query;
        }


        /// <summary>
        /// Function which returns all found records, could be filtered by userId 
        /// </summary>
        /// <param name="userId">default null</param>
        /// <param name="noTracking">default true</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<TDALEntity?>> GetAllAsync(object? userId = null, bool noTracking = true)
        {
            // Prepare query
            var query = PrepareQuery(userId, noTracking);

            var domainEntities = await query.ToListAsync();
            var result = domainEntities.Select(e => Mapper.Map(e));
            return result;

        }

        /// <summary>
        /// Function which returns first or default found entity by id.
        /// </summary>
        /// <param name="id">Id to look for</param>
        /// <param name="userId">default null</param>
        /// <param name="noTracking">default true</param>
        /// <returns></returns>
        public virtual async Task<TDALEntity?> FirstOrDefaultAsync(TKey id, object? userId = null,
            bool noTracking = true)
        {
            // Prepare query
            var query = PrepareQuery(userId, noTracking);

            var domainEntity  = await query.FirstOrDefaultAsync(e => e.Id.Equals(id));
            var result = Mapper.Map(domainEntity);
            return result;
        }

        /// <summary>
        /// Function which adds entities to DbSet
        /// </summary>
        /// <param name="entity">Entity to add</param>
        /// <returns>Tracked entity</returns>
        public virtual TDALEntity Add(TDALEntity entity)
        {
            var domainEntity = Mapper.Map(entity);
            var trackedDomainEntity = RepoDbSet.Add(domainEntity).Entity;
            RepoDbContext.AddToEntityTracker(trackedDomainEntity, entity);
            var result = Mapper.Map(trackedDomainEntity);
            return result;
        }

        /// <summary>
        /// Updates entity
        /// </summary>
        /// <param name="entity">Entity to update</param>
        /// <param name="userId">default =null</param>
        /// <returns>Updated entity</returns>
        public virtual async Task<TDALEntity> UpdateAsync(TDALEntity entity, object? userId = null)
        {
            var domainEntity = Mapper.Map(entity);
            await CheckDomainEntityOwnership(domainEntity, userId);
            var trackedDomainEntity = RepoDbSet.Update(domainEntity).Entity;
            var result = Mapper.Map(trackedDomainEntity);
            return result;

        }

        /// <summary>
        /// Removes entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="userId">default =null</param>
        /// <returns>Deleted entity</returns>
        public virtual async Task<TDALEntity> RemoveAsync(TDALEntity entity, object? userId = null)
        {
            var domainEntity = Mapper.Map(entity);
            await CheckDomainEntityOwnership(domainEntity, userId);
            return Mapper.Map(RepoDbSet.Remove(domainEntity).Entity);

        }

        /// <summary>
        /// Removes entity by Id
        /// </summary>
        /// <param name="id">Entity id to be deleted by</param>
        /// <param name="userId">default =null</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public virtual async Task<TDALEntity> RemoveAsync(TKey id, object? userId = null)
        {
            var query = PrepareQuery(userId);
            var domainEntity = await query.FirstOrDefaultAsync(e => e.Id.Equals(id));
            if (domainEntity == null)
            {
                throw new ArgumentException("Entity to be updated was not found in data source!");
            }
            return Mapper.Map(RepoDbSet.Remove(domainEntity).Entity);

        }

        /// <summary>
        /// Function which checks, does entity exists or not
        /// </summary>
        /// <param name="id"> Entity id to look for</param>
        /// <param name="userId">default =null</param>
        /// <returns></returns>
        public virtual async Task<bool> ExistsAsync(TKey id, object? userId = null)
        {
            var query = PrepareQuery(userId);
            return await query.AnyAsync(e => e.Id.Equals(id));
            

        }

        protected async Task CheckDomainEntityOwnership(TDomainEntity entity, object? userId = null)
        {
            var recordExists = await ExistsAsync(entity.Id, userId);
            if (!recordExists)
            {
                throw new ArgumentException("Entity to be updated was not found in data source!");
            }
        }
    }
}