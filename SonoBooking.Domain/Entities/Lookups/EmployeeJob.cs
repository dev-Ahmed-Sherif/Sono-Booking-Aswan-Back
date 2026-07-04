using SonoBooking.Domain.Entities.Base;
using System;

namespace SonoBooking.Domain.Entities.Lookups;

public class EmployeeJob : Lookup<string>
{
    public EmployeeJob()
    {
        if (string.IsNullOrEmpty(Id))
        {
            Id = Guid.CreateVersion7().ToString();
        }
    }
}
