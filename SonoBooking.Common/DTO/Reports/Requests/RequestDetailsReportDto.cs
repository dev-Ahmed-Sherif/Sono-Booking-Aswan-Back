namespace SonoBooking.Common.DTO.Reports.Requests
{
    public class RequestDetailsReportDto
    {
        public string RequestDate { get; set; }
        public string RequestDateDay { get; set; }
        public string RequestDateMonth { get; set; }
        public string RequestDateYear { get; set; }
        public string LeaderFullName { get; set; }
        public string LeaderPosition { get; set; }
        public string ApplicantName { get; set; }
        public string NationalId { get; set; }
        public string JobTitle { get; set; }
        public string Employer { get; set; }
        public string UnitGovernorate { get; set; }
        public string DestinationGovernorate { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Nights { get; set; }
        public string Apartments { get; set; }
        public string Rooms { get; set; }
        public string Beds { get; set; }
        public string IsWorkMission { get; set; }
        public string IsMedical { get; set; }
        public string IsSpecial { get; set; }
        public string Phone { get; set; }
        public string Attachments { get; set; }
        public string RejectionReason { get; set; }
        public string ApprovalNotes { get; set; }
        public string ShowApprovedStamp { get; set; }
        public string ShowRejectedStamp { get; set; }
        public string StatusUpdatedAt { get; set; }
        public byte[]? StatusUpdatedAtImage { get; set; }
        public byte[]? LeaderSignatureImage { get; set; }
        public string LeaderSignatureMimeType { get; set; }
        public string ShowLeaderSignature { get; set; }
        public byte[]? QrCodeImage { get; set; }
        public byte[]? ApprovedStampImage { get; set; }
        public byte[]? RejectedStampImage { get; set; }
        public string User { get; set; }
    }
}
