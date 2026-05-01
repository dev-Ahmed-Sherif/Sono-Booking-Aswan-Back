using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SonoTracker.Application.Services.Enums.OrganizationTypes;
using SonoTracker.Common.Core;
using SonoTracker.Domain.Enum;

namespace SonoTracker.Application.Services.Enums.IDTypes
{
    public class IDTypeService : IIDTypeService
    {
        public async Task<IFinalResult> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var enumNames = Enum.GetValues(typeof(IDType)).Cast<IDType>().Select(e => e.GetName()).ToList();
            return await Task.FromResult(new ResponseResult().PostResult(enumNames, HttpStatusCode.OK));

        }
    }
}
