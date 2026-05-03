using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using SonoBooking.Domain;
using SonoTracker.Application.Services.Enums.Genders;
using SonoTracker.Common.Core;

namespace SonoTracker.Application.Services.Enums.Genders
{
    public class GenderService : IGenderService
    {
        public async Task<IFinalResult> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var enumNames = Enum.GetValues(typeof(Gender)).Cast<Gender>().Select(e => e.GetName()).ToList();
            return await Task.FromResult(new ResponseResult().PostResult(enumNames, HttpStatusCode.OK));
        }
    

    }
}
