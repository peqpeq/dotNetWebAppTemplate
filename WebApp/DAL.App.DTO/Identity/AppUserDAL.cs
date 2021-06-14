using System;
using Microsoft.AspNetCore.Identity;

namespace DAL.App.DTO.Identity
{
    public class AppUserDAL : AppUser<Guid>
    {
    }


    public class AppUser<TKey> : IdentityUser<TKey>
        where TKey : struct, IEquatable<TKey>
    {

    }

}