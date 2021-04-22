using System;
using Contracts.Domain;

namespace Domain.Base
{
    public abstract class DomainEntityIdMetadata : DomainEntityIdMetadata<Guid>, IDomainEntityId
    {
        
    }

    public abstract class DomainEntityIdMetadata<TKey> : DomainEntityId<TKey>, IDomainEntityMetadata
        where TKey : IEquatable<TKey>
    {
        public string CreatedBy { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
    }

}