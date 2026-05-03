using SonoBooking.Domain.Entities.Lookups;
using SonoTracker.Application.Services.Base;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Lookup.Attach;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SonoTracker.Application.Services.LookUp.Attach
{
    public interface IAttachService : IBaseService<Attachment, AddAttachDto, EditAttachDto, AttachDto, string, string>
    {
        Task<IFinalResult> DeleteRangeAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
    }
}
