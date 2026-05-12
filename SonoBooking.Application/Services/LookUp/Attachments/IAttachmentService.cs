using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Lookup.Attachment;
using SonoBooking.Common.DTO.Lookup.Attachment.Parameters;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AttachmentEntity = SonoBooking.Domain.Entities.Lookups.Attachment;

namespace SonoBooking.Application.Services.LookUp.Attachments
{
    public interface IAttachmentService : IBaseService<AttachmentEntity, AddAttachmentDto, EditAttachmentDto, AttachmentDto, string, string>
    {
        Task<PagingResult> GetAllPagedAsync(BaseParam<AttachmentFilter> filter, CancellationToken cancellationToken = default);

        Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default);

        Task<IFinalResult> DeleteRangeAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
    }
}
