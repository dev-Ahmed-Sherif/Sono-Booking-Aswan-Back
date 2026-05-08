using SonoBooking.Application.Services.Base;
using SonoBooking.Common.DTO.Lookup.Attachment;
using SonoBooking.Common.DTO.Lookup.Attachment.Parameters;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using AttachmentEntity = SonoBooking.Domain.Entities.Lookups.Attachment;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.LookUp.Attachment
{
    public interface IAttachService : IBaseService<AttachmentEntity, AddAttachmentDto, EditAttachmentDto, AttachmentDto, string, string>
    {
        Task<PagingResult> GetAllPagedAsync(BaseParam<AttachmentFilter> filter, CancellationToken cancellationToken = default);

        Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default);
    }
}
