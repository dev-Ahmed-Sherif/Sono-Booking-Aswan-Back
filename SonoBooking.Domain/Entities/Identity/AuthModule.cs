using SonoBooking.Domain.Entities.Base;
using System;
using System.Collections.Generic;

namespace SonoTracker.Domain.Entities.Identity
{
    public class AuthModule : Lookup<string>
    {
        public AuthModule()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.CreateVersion7().ToString();
            }
        }

        public virtual HashSet<AuthPage> AuthPages { get; set; } = [];
    }
}
