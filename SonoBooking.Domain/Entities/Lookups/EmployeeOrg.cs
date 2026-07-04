using SonoBooking.Domain.Entities.Base;
using System;

namespace SonoBooking.Domain.Entities.Lookups;

public class EmployeeOrg : Lookup<string>
{
    public EmployeeOrg()
    {
        if (string.IsNullOrEmpty(Id))
        {
            Id = Guid.CreateVersion7().ToString();
        }
    }
}
