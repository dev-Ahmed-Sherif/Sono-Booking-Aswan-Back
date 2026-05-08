using SonoBooking.Common.Core;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Enums.AllocationTypes
{
    public interface IAllocationTypeService
    {
        Task<IFinalResult> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
