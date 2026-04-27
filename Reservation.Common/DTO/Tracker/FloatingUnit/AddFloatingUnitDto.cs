using Microsoft.AspNetCore.Http;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace SonoTracker.Common.DTO.Tracker.FloatingUnit
{
    [ExcludeFromCodeCoverage]

    public class AddFloatingUnitDto : LookupDto<string>, IValidatableObject
    {
        private static readonly string[] MaintenanceDateFormats = ["dd/MM/yyyy", "dd-MM-yyyy"];


        [MaxLength(20), Required, RegularExpression("^[0-9]{1,20}$",
         ErrorMessage = "License Number must be numeric and up to 20 digits")]
        public required string LicenseNumber { get; set; }

        [Required, Range(0.01f, 99999f,
         ErrorMessage = "Length must be greater than 0 and less than or equal to 99999")]
        public required float Length { get; set; }

        [Required, Range(0.01f, 99999f,
         ErrorMessage = "Width must be greater than 0 and less than or equal to 99999")]
        public required float Width { get; set; }

        [Required, Range(0, 99999,
         ErrorMessage = "Passenger number must be between 0 and 99999")]
        public required int PassengerNumber { get; set; }

        [Required, Range(0, 99999,
         ErrorMessage = "Room number must be between 0 and 99999")]
        public required int RoomNumber { get; set; }

        [Required]
        public required DateOnly ManufactureYear { get; set; }

        [MaxLength(10)]
        [RegularExpression(
            "^$|^(0[1-9]|[12][0-9]|3[01])([-/])(0[1-9]|1[0-2])\\2\\d{4}$",
            ErrorMessage = "Last maintenance date must be dd/mm/yyyy or dd-mm-yyyy")]
        public string LastMaintenanceDate { get; set; }

        [MaxLength(10)]
        [RegularExpression(
            "^$|^(0[1-9]|[12][0-9]|3[01])([-/])(0[1-9]|1[0-2])\\2\\d{4}$",
            ErrorMessage = "Next maintenance date must be dd/mm/yyyy or dd-mm-yyyy")]
        public string NextMaintenanceDate { get; set; }

        [MaxLength(50), Required]
        public  required string UnitTypeId { get; set; }
        
        //[Required]
        public IFormFile ImageUrl { get; set; }

        [Required]
        public required bool IsAccepted { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrWhiteSpace(LastMaintenanceDate) &&
                !DateOnly.TryParseExact(LastMaintenanceDate.Trim(), MaintenanceDateFormats, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out _))
                yield return new ValidationResult(
                    "Last maintenance date must be dd/mm/yyyy or dd-mm-yyyy with a valid calendar date.",
                    new[] { nameof(LastMaintenanceDate) });

            if (!string.IsNullOrWhiteSpace(NextMaintenanceDate) &&
                !DateOnly.TryParseExact(NextMaintenanceDate.Trim(), MaintenanceDateFormats, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out _))
                yield return new ValidationResult(
                    "Next maintenance date must be dd/mm/yyyy or dd-mm-yyyy with a valid calendar date.",
                    new[] { nameof(NextMaintenanceDate) });
        }
    }
}
