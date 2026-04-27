using System;
using System.Diagnostics.CodeAnalysis;
using SonoTracker.Domain.Entities.Base;

namespace SonoTracker.Domain.Entities.Audit
{
    [ExcludeFromCodeCoverage]
    public class Audit : BaseEntity<string>
    {
        public Audit()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.CreateVersion7().ToString();
            }
        }

        public string UserId { get; set; }
        public string Type { get; set; }
        public string TableName { get; set; }
        public DateTime DateTime { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
        public string AffectedColumns { get; set; }
        public string PrimaryKey { get; set; }
    }
}
