using System;
using System.Globalization;
using SonoTracker.Common.DTO.Tracker.FloatingUnit;
using SonoTracker.Domain.Entities.Tracker;

namespace SonoTracker.Application.Mapping
{
    public partial class MappingService
    {
        public void MapFloatingUnit()
        {
            CreateMap<FloatingUnit, FloatingUnitDto>()
                 .ForMember(des => des.UnitCategory, opt => opt.MapFrom(src => src.UnitType.UnitCategory))
                 .ForMember(des => des.UnitType, opt => opt.MapFrom(src => src.UnitType.NameAr))
                 .ReverseMap();
            CreateMap<FloatingUnit, EditFloatingUnitDto>()
                .ForMember(des => des.UnitCategory, opt => opt.MapFrom(src => src.UnitType.UnitCategory))
                .ForMember(des => des.UnitType, opt => opt.MapFrom(src => src.UnitType.NameAr))
                .ReverseMap();

            CreateMap<AddFloatingUnitDto, FloatingUnit>()
                .ForMember(des => des.Id, opt => opt.Ignore())
                .ForMember(des => des.LastMaintenanceDate, opt => opt.MapFrom(src => ParseDdMmYyyy(src.LastMaintenanceDate)))
                .ForMember(des => des.NextMaintenanceDate, opt => opt.MapFrom(src => ParseDdMmYyyy(src.NextMaintenanceDate)));
        }

        private static DateOnly? ParseDdMmYyyy(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            var s = value.Trim();
            string[] formats = ["dd/MM/yyyy", "dd-MM-yyyy"];
            return DateOnly.TryParseExact(s, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var d)
                ? d
                : null;
        }
    }
}
