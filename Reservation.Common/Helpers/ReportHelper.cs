using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeputyOffice.Common.Helpers
{
    public static class ReportHelper
    {
        public static string GetReportDetails(string reportName, string reportType)
        {

            string outputFileName = reportType.ToUpper() switch
            {
                "EXCEL" => reportName + ".xls",
                "WORD" => reportName + ".doc",
                _ => reportName + ".pdf",
            };
            return outputFileName;
        }
    }
}
