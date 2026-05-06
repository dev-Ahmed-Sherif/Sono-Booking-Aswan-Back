using SonoBooking.Domain;
using SonoBooking.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SonoBooking.Domain.Entities.Lookups
{
    public class RequestType : Lookup<string>
    {
        public RequestType() 
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.CreateVersion7().ToString();
            }
        }
    }
}
