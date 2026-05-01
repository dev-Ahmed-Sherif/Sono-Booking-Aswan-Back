using SonoTracker.Common.Core;
using SonoTracker.Domain.Enum;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoTracker.Application.Services.Enums.UnitCategories
{
    public class UnitCategoryService : IUnitCategoryService
    {
        public async Task<IFinalResult> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var enumNames = Enum.GetValues(typeof(UnitCategory)).Cast<UnitCategory>().Select(e => e.GetName()).ToList();
            return await Task.FromResult(new ResponseResult().PostResult(enumNames, HttpStatusCode.OK));

        }
    }
}
