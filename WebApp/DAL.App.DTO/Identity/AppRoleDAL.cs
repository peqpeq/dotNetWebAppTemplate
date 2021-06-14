using System;
using Microsoft.AspNetCore.Identity;

namespace DAL.App.DTO.Identity
{
    public class AppRoleDAL : IdentityRole<Guid>
    {
    }

    public class AppRole<TKey> : IdentityRole<TKey> 
        where TKey : IEquatable<TKey>
    {
        
    }

}