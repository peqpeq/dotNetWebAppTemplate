using System;
using Microsoft.AspNetCore.Identity;

namespace Domain.App.Identity
{
    public class AppRole : IdentityRole<Guid>
    {
    }

    public class AppRole<TKey> : IdentityRole<TKey> 
        where TKey : IEquatable<TKey>
    {
        
    }

}