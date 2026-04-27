using System.Threading;
using System.Threading.Tasks;
using SonoTracker.Common.Core;

namespace SonoTracker.Application.Services.Enums.UnitCategories
{
    public interface IUnitCategoryService
    {
        Task<IFinalResult> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
