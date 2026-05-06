using AutoMapper;

namespace SonoTracker.Application.Mapping
{
    public partial class MappingService : Profile
    {
        public MappingService()
        {
            MapAccidentType();
            MapCity();
            MapTown();
            MapUnitType();
            MapNationality();
        }
    }
}