using SonoTracker.Domain.Entities.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SonoTracker.Domain.Entities.Identity
{
    public class AuthPage : Lookup<string>
    {
        public AuthPage()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.CreateVersion7().ToString();
            }
            AuthModule = null!;
        }

        [MaxLength(50)]
        [ForeignKey(nameof(AuthModule))]
        public string? AuthModuleId { get; set; }
        public virtual AuthModule? AuthModule { get; set; }
    }
}
