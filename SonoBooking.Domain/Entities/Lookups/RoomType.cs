using SonoBooking.Domain.Entities.Base;
using System;

namespace SonoBooking.Domain.Entities.Lookups
{
    public class RoomType : Lookup<string>
    {
        public RoomType() 
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.CreateVersion7().ToString();
            }
        }
    }
}
