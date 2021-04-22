using System;

namespace Contracts.Domain
{
    public interface IDomainEntityMetadata
    {
        string CreatedBy { get; set; }
        DateTime CreatedAt { get; set; }
    }

}