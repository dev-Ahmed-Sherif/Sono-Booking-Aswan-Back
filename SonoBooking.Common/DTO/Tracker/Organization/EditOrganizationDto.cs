using System;
using System.Diagnostics.CodeAnalysis;
using SonoTracker.Common.Core;
using SonoTracker.Domain.Enum;

namespace SonoTracker.Common.DTO.Tracker.Organization
{
    [ExcludeFromCodeCoverage]
    public class EditOrganizationDto : IEntityDto<string>
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string NationalityId { get; set; }
        public string NationalityName { get; set; }
        public OrganizationType OrganizationType { get; set; }
        //public EnumResult OrganizationType { get; set; }
        public string InspectionTypeId { get; set; }
        public string InspectionType { get; set; }
        public DateTime CreationDate { get; set; }
        public string CommercialRegistrationNumber { get; set; }
        public string CommercialRegistrationAttachment { get; set; }
        public string WebSiteAddress { get; set; }
        public string TouristMarinaNumber { get; set; }
        public bool IsAccepted { get; set; }
        public bool IsReport { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedById { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedById { get; set; }
    }
}

