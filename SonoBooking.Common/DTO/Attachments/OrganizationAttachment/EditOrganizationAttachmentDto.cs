using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SonoBooking.Common.Core;

namespace SonoBooking.Common.DTO.Attachments.OrganizationAttachment
{
    public class EditOrganizationAttachmentDto : IEntityDto<string>
    {
        public string? Id { get; set; }

        public Guid FileId { get; set; }

        public string FileName { get; set; }

        public string Extension { get; set; }

        public string Size { get; set; }

        public bool IsPublic { get; set; }

        public string Url { get; set; }

        public string AttachmentDisplaySize { get; set; }

        public Guid OrganizationId { get; set; }
    }
}


