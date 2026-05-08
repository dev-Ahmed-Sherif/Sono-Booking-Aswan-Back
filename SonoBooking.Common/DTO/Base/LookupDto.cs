using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using SonoBooking.Common.Core;

namespace SonoBooking.Common.DTO.Base
{
    [ExcludeFromCodeCoverage]
    public class LookupDto<T> : IEntityDto<T>
    {
        public T Id { get; set; }
        public string Code { get; set; }

        [Required, RegularExpression("^[\u0600-\u06FF\\-\\s]+$", 
         ErrorMessage = "NameAr should Arabic Letters Only No Numbers Or Special Chars Allowed")]
        public string NameAr { get; set; }

        //[Required, RegularExpression("^[a-zA-Z\\-\\s]+$",
        // ErrorMessage = "NameEn should be English Letters Only No Numbers Or Special Chars Allowed")]
        [RegularExpression("^[a-zA-Z\\-\\s]+$",
         ErrorMessage = "NameEn should be English Letters Only No Numbers Or Special Chars Allowed")]
        public string NameEn { get; set; }

    }
}


