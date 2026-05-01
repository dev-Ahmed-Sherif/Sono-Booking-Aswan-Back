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
            MapRoute();
            MapMaintenanceType();
            MapUnitType();
            MapOrganizationCategory();
            MapNationality();
            MapInspectionType();
            MapGeoPoint();
            MapGovernorate();
            MapOrganization();
            MapOrganizationStaff();
            MapFloatingUnit();
            MapFloatingUnitStaff();
            MapTripInformation();
            MapFloatingUnitOrganization();
            MapInspection();
            MapTouristMarina();
            MapMarinaOrganization();
            MapLicenseApplication();
            MapTripGeo();
            MapMarinaTrip();
            MapMaintenance();
            MapAccident();
            MapUser();
            MapInspectionClause();
            MapInspectionFloatingUnitClause();
        }
    }
}