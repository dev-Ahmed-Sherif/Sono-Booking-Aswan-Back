using SonoBooking.Domain;
using SonoBooking.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SonoBooking.Domain.Entities.Lookups
{
    public class ApartmentType : Lookup<string>
    {
        public ApartmentType() 
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.CreateVersion7().ToString();
            }
        }
    }
}
