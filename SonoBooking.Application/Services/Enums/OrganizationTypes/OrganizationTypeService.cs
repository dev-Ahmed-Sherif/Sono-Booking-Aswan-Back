using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using SonoBooking.Domain;
using SonoTracker.Common.Core;

namespace SonoTracker.Application.Services.Enums.OrganizationTypes
{
    public class OrganizationTypeService : IOrganizationTypeService
    {
        public async Task<IFinalResult> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var enumNames = Enum.GetValues(typeof(OrganizationType)).Cast<OrganizationType>().Select(e => e.GetName()).ToList();
            return await Task.FromResult(new ResponseResult().PostResult(enumNames, HttpStatusCode.OK));

        }
    }
}