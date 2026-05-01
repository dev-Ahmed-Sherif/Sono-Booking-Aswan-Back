using System;
using System.Diagnostics.CodeAnalysis;
using SonoTracker.Domain.Enum;


namespace SonoTracker.Domain.Enum
{
    #region SonoTracker Enum
    public enum TicketCreatorType
    {
        [Values("None", "لا يوجد", "NONE")]
        None = 0,
        [Values("Employee", "موظف", "EMPLOYEE")]
        Employee = 1,
        [Values("Citizen", "مواطن", "CITIZEN")]
        Citizen = 4,
    }
    public enum TicketActionType
    {
        [Values("None", "لا يوجد", "NONE")]
        None = 0,
        [Values("Submit", "تم الارسال", "SUBMIT")]
        Submit = 1,
        [Values("Reply", "تم الرد", "REPLY")]
        Reply = 2,
        [Values("RequestFurtherInformation", "طلب معلومات", "REQUESTFURTHERINFORMATION")]
        RequestFurtherInformation = 3,
        [Values("Close", "مغلق", "CLOSED")]
        Close = 4
    }
    public enum TicketSource
    {
        None = 0,
        MobileAPP = 1,
        Website = 2,
        Bot = 3,
        Chat = 4,
        CallCenter = 5,
        Phone = 6,
        Mobile = 7,
        BySystem = 8,
    }
    public enum TicketStatus
    {
        [Values("Undefined", "غير معروف", "UNDEFINED")]
        Undefined = 0,
        [Values("Submitted", "تم إرسال المراجعة", "SUBMITTED")]
        Submitted = 1,
        [Values("InProgress", "تم فتح المراجعة واتخاذ إجراء", "INPROGRESS")]
        InProgress = 2,
        [Values("NeedCustomerResponse", "تحتاج لرد من المرسل", "NEEDCUSTOMERRESPONSE")]
        NeedCustomerResponse = 3,
        [Values("Closed", "مغلقة/الموظف قام برد نهائى على المراجعة", "CLOSED")]
        Closed = 4,
        [Values("InInternalProgress", "جارى المعالجة/الموظف رفعها لإدارة أعلى منه", "ININTERNALPROGRESS")]
        InInternalProgress = 5
    }
    public enum InternallyProcessingType
    {
        None = 0,
        Workflow = 1,
        CRS = 2,
    }
    public enum TicketRequestStatus
    {
        None = 0,
        Open = 1,
        InProgress = 2,
        Closed = 3,
    }
    public enum CustomerTicketRequestStatus
    {
        [Values("Sent", "الصادر", "SENT")]
        Sent = 0,
        [Values("Inbox", "الوارد", "INBOX")]
        Inbox = 1,
        [Values("Close", "المغلقة", "CLOSE")]
        Closed = 2,
    }
    public enum TicketFileType
    {
        PDF = 1,
        Image = 2,
        Word = 3,
        Excel = 4,
        PowerPoint = 5,
        Other = 0,
    }
    public enum DocumentClassValueItemType
    {
        None = 0,
        CRSHistory = 1,
        Attachment = 2,
        HelpDesk = 3,
    }
    public enum CitizenBrowseModes
    {
        None = 0,
        DisplayTicketDetailsOnly = 1,
        DisplayChart = 2,
        DisplayTracking = 3
    }
    public enum NotifyCitizenMethods
    {
        None = 0,
        Email = 1,
        SMS = 2,
        Both = 3
    }
    public enum NotifyCitizenRepeatLocations
    {
        None = 0,
        BeginAndEnd = 1,
        EachStep = 2
    }
    public enum DocumentTypeCategory
    {
        None = 0,
        HelpDesk = 1,
    }
    public enum WorkflowTaskDecision
    {
        Undefined = 0,
        Accepeted = 1,
        Refused = 2,
        Returned = 3,
        Reviewed = 4,
        GetAdvice = 5
    }
    public enum WorkflowStatus
    {
        Active = 1,
        Inactive = 2,
        Pending = 3,
        Terminated = 4,
        Completed = 5,

    }

    public enum UserType
    {
        [Values("Administrator", "مدير النظام", "ADMINISTRATOR")]
        Administrator = 0,
        [Values("Employee", "موظف", "EMPLOYEE")]
        Employee = 1,
        [Values("Citizen", "مواطن", "CITIZEN")]
        Citizen = 4,
    }
    public enum SMSGateway
    {
        Undefined = 0,
        TamimahSMSGateway = 1
    }
    public enum WatermarkTypes
    {
        None = 0,
        UserName = 1, // username - Display name printed at yyyy-mm-dd 04:35 PM
        UserID = 2
    }
    public enum CurrentUserHeaderTypes
    {
        Plain = 0,
        Encrypted = 1,
        Token = 2
    }
    public enum LandingTheme
    {
        Default = 0,
        Dhow = 1,
        Metro = 2
    }
    public enum NotificationCategories
    {
        Workflows,
        System,
        Admin
    }
    public enum AttachmentType
    {
        [Values("Ticket", "المراجعة", "")]
        Ticket = 0,
        [Values("TicketResponse", "الرد على المراجعة", "")]
        TicketResponse = 1
    }
    public enum UnitCategory
    {
        [Values("FixedUnit", "وحدات ثابتة", "FixedUnit")]
        FixedUnit = 1,
        [Values("MovableUnit", "وحدات متحركة", "MovableUnit")]
        MovableUnit = 2
    }

    public enum OrganizationType
    {
        [Values("OwnerCompany", "شركة مالكة", "OwnerCompany")]
        OwnerCompany = 1,
        [Values("OperatingCompany", "شركة مشغلة", "OperatingCompany")]
        OperatingCompany = 2,
        [Values("GovernmentCompany", "جهة حكومية مسؤولة", "GovernmentCompany")]
        GovernmentCompany = 3
    }

    #endregion


    #region Lookup Enum

    public enum Gender
    {
        [Values("Male", "ذكر", "Male")]
        Male = 1,
        [Values("Female", "أنثى", "Female")]
        Female = 2,
        //[Values("Both", "الكل", "Both")]
        //Both
    }
    
    public enum Status
    {
        [Values("Approved", "مقبول", "APPROVED")]
        Approved = 1,
        [Values("NeedCompelete", "مطلوب الإستكمال", "NEEDCOMPELETE")]
        NeedCompelete = 2,
        [Values("Pending", "جارى المراجعة", "PENDING")]
        Pending = 3
    }
    public enum Case
    {
        [Values("InProgress", "جاري الأصلاح", "InProgress")]
        InProgress = 1,
        [Values("Implemented", "تم الأصلاح", "Implemented")]
        Implemented = 2
    }
    public enum IDType
    {
        [Values("IDCard", "بطاقة شخصية", "IDCard")]
        IDCard = 1,
        [Values("Passport", "جواز سفر", "Passport")]
        Passport = 2
    }


    #endregion


    #region Common Enum

    public enum AuditType
    {
        None = 0,
        Create = 1,
        Update = 2,
        Delete = 3
    }
    //public enum UnitType
    //{
    //    Sector = 1,
    //    Directorate,
    //    Department,
    //    Section,
    //    Team
    //}
    public enum MaritalStatus
    {
        Single = 1,
        Married,
        Divorced,
        Widow
    }

    #endregion

    [ExcludeFromCodeCoverage]
    internal class Values : Attribute
    {
        public string NameEn;
        public string NameAr;
        public string Code;
        public Values(string nameEn, string nameAr, string code)
        {
            NameAr = nameAr;
            NameEn = nameEn;
            Code = code;
        }
    }
}
[ExcludeFromCodeCoverage]
public static class Extensions
{
    public static ActionResult GetActionName(this System.Enum e)
    {
        var type = e.GetType();

        var memInfo = type.GetMember(e.ToString());

        if (memInfo.Length > 0)
        {
            var attrs = memInfo[0].GetCustomAttributes(typeof(Values), false);
            if (attrs.Length > 0)
            {
                var attributes = (Values)attrs[0];
                return new ActionResult
                {
                    Id = Convert.ToInt32(e),
                    NameEn = attributes.NameEn,
                    NameAr = attributes.NameAr,
                    Code = attributes.Code
                };
            }
        }

        throw new ArgumentException("Name " + e + " has no Name defined!");
    }
    public static EnumResult GetName(this Enum e)
    {
        var type = e.GetType();

        var memInfo = type.GetMember(e.ToString());

        if (memInfo.Length > 0)
        {
            var attrs = memInfo[0].GetCustomAttributes(typeof(Values), false);
            if (attrs.Length > 0)
            {
                var attributes = (Values)attrs[0];
                return new EnumResult
                {
                    Id = Convert.ToInt32(e),
                    NameEn = attributes.NameEn,
                    NameAr = attributes.NameAr,
                    Code = attributes.Code
                };
            }
        }

        throw new ArgumentException("Name " + e + " has no Name defined!");
    }
}

[ExcludeFromCodeCoverage]
public class EnumResult
{
    public int Id { get; set; }
    public string NameEn { get; set; }
    public string NameAr { get; set; }
    public string Code { get; set; }
}

[ExcludeFromCodeCoverage]
public class ActionResult
{
    public int Id { get; set; }
    public string NameEn { get; set; }
    public string NameAr { get; set; }
    public string Code { get; set; }
}

