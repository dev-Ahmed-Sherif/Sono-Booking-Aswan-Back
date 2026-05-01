using SonoTracker.Common.Core;
using SonoTracker.Domain.Enum;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SonoTracker.Common.DTO.Tracker.FloatingUnitStaff
{
    [ExcludeFromCodeCoverage]

    public class FloatingUnitStaffDto : IEntityDto<string>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Job { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public Gender Gender { get; set; }
        public IDType IDType { get; set; }
        public string Identity { get; set; }
        public string NationalityId { get; set; }
        public string NationalityNameAr { get; set; }
        public string FloatingUnitId { get; set; }
        public bool IsDelegate { get; set; }
        public string DelegateAttachment { get; set; }
        public bool IsDeleted { get; set; }
    }
}
