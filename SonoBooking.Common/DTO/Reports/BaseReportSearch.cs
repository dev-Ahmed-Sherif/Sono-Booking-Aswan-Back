using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonoBooking.Common.DTO.Reports
{
    public class BaseReportSearch
    {
        [Required]
        public required string ReportName { get; set; }
        [Required]
        public required string ReportType { get; set; }
    }
}

