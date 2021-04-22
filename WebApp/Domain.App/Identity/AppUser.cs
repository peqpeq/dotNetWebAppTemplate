using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Domain.App.Identity
{
    public class AppUser : AppUser<Guid>
    {
    }


    public class AppUser<TKey> : IdentityUser<TKey>
        where TKey : struct, IEquatable<TKey>
    {

        // add your own fields

        //Properties
        public string Name { get; set; } = default!;

        public string? Gender { get; set; }
        
        public string? AvatarImg { get; set; }
        

        //FK
        
        public ICollection<AppRole>? Roles { get; set; }
        

    }

}