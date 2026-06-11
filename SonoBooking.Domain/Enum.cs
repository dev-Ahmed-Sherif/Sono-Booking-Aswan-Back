using System;
using System.Diagnostics.CodeAnalysis;
using SonoBooking.Domain;

namespace SonoBooking.Domain
{
    #region SonoBooking Enum
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
        [Values("Pending", "معلق", "PENDING")]
        NeedCompelete = 2,
        [Values("Rejected", "مرفوض", "REJECTED")]
        Pending = 3,
        [Values("Canceled", "ملغى", "CANCELED")]
        Canceled = 4
    }

    public enum UnitStatus
    {
        [Values("Available", "متاح", "AVAILABLE")]
        Available = 1,
        [Values("Reserved", "محجوز", "RESERVED")]
        Reserved = 2,
        [Values("Occupied", "مشغول", "OCCUPIED")]
        Occupied = 3
    }

    public enum ReservationStatus
    {
        [Values("Reserved", "محجوز", "RESERVED")]
        Reserved = 1,
        //[Values("CheckedIn", "تأكيد وصول", "CHECKEDIN")]
        //CheckedIn = 2,
        [Values("Completed", "تم اكتمال الاقامة", "COMPLETED")]
        Completed = 2,
        [Values("Canceled", "ملغى", "CANCELED")]
        Canceled = 3,
        [Values("NoShow", "لم يظهر", "NOSHOW")]
        NoShow = 4,
        [Values("Checkout", "مقبول", "CHECKOUT")]
        Checkout = 5
    }

    public enum PaymentStatus
    {
        [Values("Pending", "معلق", "PENDING")]
        Pending = 1,
        [Values("Paid", "مدفوع", "PAID")]
        Paid = 2,
        [Values("Failed", "فشل", "FAILED")]
        Failed = 3,
        [Values("Refunded", "مسترجع", "REFUNDED")]
        Refunded = 4
    }

    public enum PaymentMethod
    {
        [Values("Cash", "نقدي", "CASH")]
        Cash = 1,
        [Values("Card", "بطاقة", "CARD")]
        Card = 2,
        [Values("Transfer", "تحويل", "TRANSFER")]
        BankTransfer = 3
    }
    public enum AllocationType
    {
        [Values("Fixed", "ثابت", "FIXED")]
        Fixed = 1,
        [Values("Flexible", "مرن", "FLEXIBLE")]
        Flexible = 2
    }

    public enum RequestCatagory
    {
        [Values("NewStay", "طلب إقامة جديد", "NEW_STAY")]
        NewStay = 1,
        [Values("Extension", "طلب تمديد", "EXTENSION")]
        Extension = 2
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
        Passport = 2,
        [Values("ResidencePermit", "شهادة ميلاد", "ResidencePermit")]
        ResidencePermit = 3
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
    public static ActionResult GetActionName(this Enum e)
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

