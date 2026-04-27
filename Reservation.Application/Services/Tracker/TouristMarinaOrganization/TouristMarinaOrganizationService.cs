using LinqKit;
using SonoTracker.Application.Services.Base;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Tracker.TouristMarina;
using SonoTracker.Common.DTO.Tracker.TripInformation.Parameters;
using SonoTracker.Common.DTO.Tracker.TripInformation;
using SonoTracker.Domain;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SonoTracker.Common.DTO.Tracker.TouristMarina.Parameters;
using Microsoft.EntityFrameworkCore;
using SonoTracker.Common.DTO.Lookup.Town;
using SonoTracker.Common.DTO.Reports.TouristMarina;
using SonoTracker.Common.DTO.Reports.Org;
using Microsoft.Reporting.NETCore;
using SonoTracker.Common.DTO.Tracker.FloatingUnitOrganization.Parameters;
using SonoTracker.Domain.Enum;
using System.Reflection;
using System.Threading;
using SonoTracker.Common.DTO.Tracker.TouristMarinaOrganization.Parameters;
using SonoTracker.Common.DTO.Tracker.TouristMarinaOrganization;
using SonoTracker.Common.DTO.Tracker.Organization;
using SonoTracker.Common.Helpers;
using SonoTracker.Application.Services.Tracker.Organizations;

namespace SonoTracker.Application.Services.Tracker.TouristMarinaOrganization
{
    public class TouristMarinaOrganizationService : BaseService<Entities.Tracker.TouristMarinaOrganization, AddTouristMarinaOrganizationDto, EditTouristMarinaOrganizationDto, TouristMarinaOrganizationDto, string, string>, ITouristMarinaOrganizationService
    {
        private readonly IOrganizationService _organizationService;

        public TouristMarinaOrganizationService(
            IServiceBaseParameter<Entities.Tracker.TouristMarinaOrganization> businessBaseParameter,
            IOrganizationService organizationService) : base(businessBaseParameter)
        {
            _organizationService = organizationService;
        }
        public override async Task<IFinalResult> GetByIdForEditAsync(object id, CancellationToken cancellationToken = default)
        {
            var idStr = id?.ToString();
            var entity = await UnitOfWork.Repository.FirstOrDefaultAsync(x => x.Id == idStr, include: src => src
                .Include(t => t.Organization)
                .Include(x => x.TouristMarina)
                , cancellationToken: cancellationToken);
            var mapped = Mapper.Map<Entities.Tracker.TouristMarinaOrganization, EditTouristMarinaOrganizationDto>(entity);
            return ResponseResult.PostResult(mapped, HttpStatusCode.OK);
        }

        public override async Task<IFinalResult> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            var idStr = id?.ToString();
            var entity = await UnitOfWork.Repository.FirstOrDefaultAsync(x => x.Id == idStr,
                include: src => src
                .Include(t => t.Organization)
               .Include(x => x.TouristMarina), cancellationToken: cancellationToken);
            var mapped = Mapper.Map<Entities.Tracker.TouristMarinaOrganization, TouristMarinaOrganizationDto>(entity);

            return ResponseResult.PostResult(mapped, HttpStatusCode.OK);
        }
        public override async Task<IFinalResult> GetAllAsync(bool disableTracking = false, Expression<Func<Entities.Tracker.TouristMarinaOrganization, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            var entity = await UnitOfWork.Repository.GetAllAsync(include: src => src
             .Include(t => t.Organization)
             .Include(x => x.TouristMarina), cancellationToken: cancellationToken);
            var filteredEntities = IsSuperAdmin()
                ? entity
                : entity.Where(e => !e.IsDeleted);
            var mapped = Mapper.Map<IEnumerable<Entities.Tracker.TouristMarinaOrganization>, IEnumerable<TouristMarinaOrganizationDto>>(filteredEntities);
            return ResponseResult.PostResult(mapped, status: HttpStatusCode.OK,
                message: HttpStatusCode.OK.ToString());
        }
        public async Task<PagingResult> GetAllPagedAsync(BaseParam<TouristMarinaOrganizationFilter> filter, CancellationToken cancellationToken = default)
        {
            var isSuperAdmin = IsSuperAdmin();
            var marinaOrganizationFilter = filter?.Filter ?? new TouristMarinaOrganizationFilter();
            if (!isSuperAdmin)
                marinaOrganizationFilter.IsDeleted = false;

            var limit = filter.PageSize;

            var offset = --filter.PageNumber * filter.PageSize;

            var query = await UnitOfWork.Repository.FindPagedAsync(predicate: PredicateBuilderFunction(marinaOrganizationFilter),
                    pageNumber: offset,
                    pageSize: limit,
                    filter.OrderByValue,
                    include: src => src
                    .Include(t => t.Organization)
                    .Include(x => x.TouristMarina),
                    cancellationToken: cancellationToken);

            var data = Mapper.Map<IEnumerable<Entities.Tracker.TouristMarinaOrganization>, IEnumerable<TouristMarinaOrganizationDto>>(query.Item2);

            return new PagingResult(filter.PageNumber, filter.PageSize, query.Item1, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }
        public async Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default)
        {
            var isSuperAdmin = IsSuperAdmin();
            var limit = filter.PageSize;

            var offset = --filter.PageNumber * filter.PageSize;

            var predicate = DropDownPredicateBuilderFunction(filter.Filter);

            var query = await UnitOfWork.Repository.FindPagedAsync(predicate: predicate, pageNumber: offset, pageSize: limit, cancellationToken: cancellationToken);

            var items = isSuperAdmin ? query.Item2 : query.Item2.Where(x => x.IsDeleted != true);
            var data = Mapper.Map<IEnumerable<Entities.Tracker.TouristMarinaOrganization>, IEnumerable<TouristMarinaOrganizationDto>>(items);

            return new PagingResult(filter.PageNumber, filter.PageSize, query.Item1, data, status: HttpStatusCode.OK, MessagesConstants.Success);

        }


        static Expression<Func<Entities.Tracker.TouristMarinaOrganization, bool>> PredicateBuilderFunction(TouristMarinaOrganizationFilter filter)
        {
            var predicate = PredicateBuilder.New<Entities.Tracker.TouristMarinaOrganization>(x => x.IsDeleted == filter.IsDeleted);

            //if (filter.TownId.HasValue)
            //{
            //    predicate = predicate.And(x => x.TownId == filter.TownId.Value);
            //}
            //if (!string.IsNullOrWhiteSpace(filter.Name))
            //{
            //    predicate = predicate.And(x => x.Name.Contains(filter.Name));
            //}
            //if (!string.IsNullOrWhiteSpace(filter.Code))
            //{
            //    predicate = predicate.And(x => x.Code.Contains(filter.Code));
            //}

            return predicate;
        }
        static Expression<Func<Entities.Tracker.TouristMarinaOrganization, bool>> DropDownPredicateBuilderFunction(SearchCriteriaFilter filter)
        {
            var predicate = PredicateBuilder.New<Entities.Tracker.TouristMarinaOrganization>(true);
            if (!string.IsNullOrWhiteSpace(filter?.SearchCriteria))
            {

                //  predicate = predicate.Or(b => b.Name.Contains(filter.SearchCriteria));
            }
            return predicate;
        }

        public async Task<IFinalResult> DeleteRangeAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
        {
            var idsList = ids.ToList();
            var entitiesToDelete = await UnitOfWork.Repository.FindAsync(d => idsList.Contains(d.Id), cancellationToken: cancellationToken);

            UnitOfWork.Repository.RemoveRange(entitiesToDelete);

            var rows = await UnitOfWork.SaveChangesAsync(cancellationToken);

            return ResponseResult.PostResult(result: rows, status: HttpStatusCode.NoContent, message: MessagesConstants.DeleteSuccess);
        }

        public override async Task<IFinalResult> AddAsync(AddTouristMarinaOrganizationDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                IFinalResult organizationResult = await _organizationService.GetByIdAsync(model.OrganizationId, cancellationToken);
                
                if (organizationResult.Data is not OrganizationDto organizationDto)
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.BadRequest, exception: null,
                                                     message: MessagesConstants.NotFound);

                IEnumerable<Entities.Tracker.TouristMarinaOrganization> existing = await UnitOfWork.Repository.FindAsync(
                    predicate: x => x.IsDeleted != true,
                    disableTracking: true,
                    cancellationToken: cancellationToken);

                IEnumerable<Entities.Tracker.TouristMarinaOrganization> existingByOrganization = await UnitOfWork.Repository.FindAsync(
                    predicate: x => x.OrganizationId == model.OrganizationId && x.IsDeleted != true,
                    disableTracking: true,
                    cancellationToken: cancellationToken);

                int organizationRecordsCount = existingByOrganization?.Count() ?? 0;
                
                if (!int.TryParse(organizationDto.TouristMarinaNumber, out int touristMarinaLimit))
                    touristMarinaLimit = 0;

                if (LookupDuplicateGuard.HasFuzzyCodeDuplicate(existing, x => x.LicenseNumber, model.LicenseNumber))
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.Conflict, exception: null,
                                                     message: MessagesConstants.Existed);

                if (touristMarinaLimit > 0 && organizationRecordsCount >= touristMarinaLimit)
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.Conflict, exception: null,
                                                     message: MessagesConstants.Existed);

                Entities.Tracker.TouristMarinaOrganization entity = Mapper.Map<Entities.Tracker.TouristMarinaOrganization>(model);
                
                await UnitOfWork.Repository.AddAsync(entity, cancellationToken);

                int affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);
                
                if (affectedRows < 0)
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.BadRequest, exception: null,
                                                     message: MessagesConstants.AddError);

                return ResponseResult.PostResult(result: entity.Id, status: HttpStatusCode.Created, exception: null,
                                                 message: MessagesConstants.AddSuccess);
            }
            catch (Exception ex)
            {
                return ResponseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: ex,
                                                 message: MessagesConstants.AddError + ex.Message);
            }
        }

        public override async Task<IFinalResult> UpdateAsync(AddTouristMarinaOrganizationDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                IEnumerable<Entities.Tracker.TouristMarinaOrganization> existing = await UnitOfWork.Repository.FindAsync(
                    predicate: x => x.Id != model.Id && x.IsDeleted != true,
                    disableTracking: true,
                    cancellationToken: cancellationToken);

                if (LookupDuplicateGuard.HasFuzzyCodeDuplicate(existing, x => x.LicenseNumber, model.LicenseNumber))
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.Conflict, exception: null,
                                                     message: MessagesConstants.Existed);

                Entities.Tracker.TouristMarinaOrganization entityToUpdate = await UnitOfWork.Repository.FirstOrDefaultAsync(x => x.Id == model.Id, cancellationToken: cancellationToken);

                Entities.Tracker.TouristMarinaOrganization entity = Mapper.Map(model, entityToUpdate);

                UnitOfWork.Repository.Update(entityToUpdate,entity);

                int affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);

                if (affectedRows < 0)
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.BadRequest, exception: null,
                                                     message: MessagesConstants.AddError);

                return ResponseResult.PostResult(result: entity.Id, status: HttpStatusCode.Created, exception: null,
                                                 message: MessagesConstants.AddSuccess);
            }
            catch (Exception ex)
            {
                return ResponseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: ex,
                                                 message: MessagesConstants.AddError + ex.Message);
            }
        }

        static Expression<Func<Entities.Tracker.TouristMarinaOrganization, bool>> PredicateBuilderReportFunction(FilterTouristMarinaReportDto filter)
        {
            var predicate = PredicateBuilder.New<Entities.Tracker.TouristMarinaOrganization>(x => x.IsDeleted == filter.IsDeleted);

            if (!string.IsNullOrWhiteSpace(filter.TownId))
            {
                predicate = predicate.And(x => x.TouristMarina.CityId == filter.TownId);
            }
            if (!string.IsNullOrEmpty(filter.TouristMarinaId))
            {
                predicate = predicate.And(x => x.TouristMarina.Id == filter.TouristMarinaId);
            }
            if (!string.IsNullOrEmpty(filter.OrganizationId))
            {
                predicate = predicate.And(x => x.Organization.Id == filter.OrganizationId);
            }

            return predicate;
        }

        public async Task<IFinalResult> GetAllReportAsync(FilterTouristMarinaReportDto filter, CancellationToken cancellationToken = default)
        {

            var query = await UnitOfWork.Repository.FindAsync(predicate: PredicateBuilderReportFunction(filter),
                 include: src => src
                   .Include(x => x.TouristMarina)
                     .Include(t => t.Organization)
                     .Include(x => x.TouristMarina.City)
                     .Include(x => x.TouristMarina.GeoPoint),
                 cancellationToken: cancellationToken);


            var data = Mapper.Map<IEnumerable<Entities.Tracker.TouristMarinaOrganization>, IEnumerable<TouristMarinaReportDto>>(query);
            return ResponseResult.PostResult(data, status: HttpStatusCode.OK,
                message: HttpStatusCode.OK.ToString());
        }

        public async Task<byte[]> GenerateReportAsync(FilterTouristMarinaReportDto filter, CancellationToken cancellationToken = default)
        {
            // get report file
            string fileDirPath = Assembly.GetExecutingAssembly().Location.Replace("SonoTracker.Application.dll", string.Empty);
            //string fileDirPath = "D:\\Projects\\SonoTracker\\Tracker\\SonoTracker.Application\\";
            //string fileDirPath = "D:\\Projects\\SonoTracker\\Tracker\\SonoTracker.Api\\bin\\Debug\\net8.0\\";
            //"D:\Projects\SonoTracker\Tracker\SonoTracker.Api\bin\Debug\net8.0\ReportsFiles";
            // Fixing unrecognized escape sequence by using verbatim string literals (prefixing with @)
            //string fileDirPath = @"D:\Projects\SonoTracker\Tracker\SonoTracker.Api\bin\Debug\net8.0\ReportsFiles";
            Console.WriteLine(string.Format(@"{0}\{1}.rdlc", fileDirPath, filter.ReportName));
            string rdclFilePath = string.Format(@"{0}ReportsFiles\{1}.rdlc", fileDirPath, filter.ReportName);


            // file encoding
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding.GetEncoding("utf-8");

            LocalReport report = new()
            {
                ReportPath = rdclFilePath
                //ReportPath = fileDirPath
            };

            // prepare data for report
            IFinalResult Org = null; // Initialize Org to avoid CS0165 error

            if (filter.ReportName == "TouristMarinasReport")
            {
                Org = await GetAllReportAsync(filter, cancellationToken);
                var orgData = Org.Data as IEnumerable<TouristMarinaReportDto>;
                if (orgData == null)
                {
                    throw new InvalidOperationException("No data found for the report.");
                }

                //Add data source to the report


                report.DataSources.Add(new ReportDataSource() { Name = "TouristMarina", Value = orgData });
            }
            

            if (Org == null || Org.Data == null)
            {
                throw new InvalidOperationException("Failed to retrieve report data.");
            }

            byte[] renderedBytes = [];
            try
            {
                renderedBytes = report.Render(filter.ReportType);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new InvalidOperationException("Error rendering report: " + ex.Message, ex);
            }

            //byte[] renderedBytes = report.Render("");

            return renderedBytes;





        }

    }
}
