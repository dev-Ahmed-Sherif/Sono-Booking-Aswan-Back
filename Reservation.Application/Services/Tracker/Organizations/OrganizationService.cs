using LinqKit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Reporting.NETCore;
using SonoTracker.Application.Services.Base;
using SonoTracker.Application.Services.Tracker.FloatingUnitOrganization;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Identity.User;
using SonoTracker.Common.DTO.Reports.Org;
using SonoTracker.Common.DTO.Tracker.FloatingUnitOrganization.Parameters;
using SonoTracker.Common.DTO.Tracker.Organization;
using SonoTracker.Common.DTO.Tracker.Organization.Parameters;
using SonoTracker.Common.Helpers;
using SonoTracker.Common.Helpers.MediaUploader;
using SonoTracker.Domain;
using SonoTracker.Domain.Entities.Base;
using SonoTracker.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SonoTracker.Domain.Entities.Tracker;

namespace SonoTracker.Application.Services.Tracker.Organizations
{
    public class OrganizationService : BaseService<Organization, AddOrganizationDto, EditOrganizationDto, OrganizationDto, string, string>, IOrganizationService
    {
        private readonly UserDataDto _user;
        private readonly IFloatingUnitOrganizationService _floatingUnitOrganizationService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IHttpContextAccessor _request;
        private readonly UploaderConfiguration _uploaderConfiguration;
        public OrganizationService(
            IServiceBaseParameter<Organization> businessBaseParameter, UserDataDto user,
            IFloatingUnitOrganizationService floatingUnitOrganizationService,
            IWebHostEnvironment hostingEnvironment, IHttpContextAccessor request) : base(businessBaseParameter)
        {
            _user = user;
            _floatingUnitOrganizationService = floatingUnitOrganizationService;
            _hostingEnvironment = hostingEnvironment;
            _request = request;
            _uploaderConfiguration = new UploaderConfiguration(_hostingEnvironment, _request);
        }

        public override async Task<IFinalResult> GetByIdForEditAsync(object id, CancellationToken cancellationToken = default)
        {
            var entity = await UnitOfWork.Repository.FirstOrDefaultAsync(x => x.Id == id.ToString(), include: src => src
                //.Include(x=>x.InspectionType)
                .Include(x => x.Nationality)
                .Include(x => x.OrganizationStaffs), cancellationToken: cancellationToken);
            var mapped = Mapper.Map<Entities.Tracker.Organization, EditOrganizationDto>(entity);

            return ResponseResult.PostResult(mapped, HttpStatusCode.OK);
        }

        public override Task<IFinalResult> GetAllAsync(bool disableTracking = false, Expression<Func<Entities.Tracker.Organization, bool>> predicate = null, CancellationToken cancellationToken = default)
            => GetAllAsync(organizationTypeId: null, organizationCategoryId: "", cancellationToken: cancellationToken);

        public async Task<IFinalResult> GetAllAsync(OrganizationType? organizationTypeId, string organizationCategoryId, CancellationToken cancellationToken = default)
        {
            var isSuperAdmin = IsSuperAdmin();
            var governorateId = isSuperAdmin ? null : GetGovernorateIdFromClaims();
            var filter = new OrganizationFilter
            {
                OrganizationType = organizationTypeId,
                OrganizationCategoryId = organizationCategoryId,
                IsDeleted = false
            };

            var entity = await UnitOfWork.Repository.FindAsync(
                predicate: PredicateBuilderFunction(filter, includeDeleted: isSuperAdmin, governorateId: governorateId),
                include: src => src
                    //.Include(t => t.InspectionType)
                    .Include(x => x.Nationality),
                cancellationToken: cancellationToken);

            var mapped = Mapper.Map<IEnumerable<Entities.Tracker.Organization>, IEnumerable<EditOrganizationDto>>(entity);
            return ResponseResult.PostResult(mapped, status: HttpStatusCode.OK, message: HttpStatusCode.OK.ToString());
        }

        public async Task<PagingResult> GetAllPagedAsync(BaseParam<OrganizationFilter> filter, CancellationToken cancellationToken = default)
        {
            var isSuperAdmin = IsSuperAdmin();
            var governorateId = isSuperAdmin ? null : GetGovernorateIdFromClaims();
            var organizationFilter = filter?.Filter ?? new OrganizationFilter();

            if (!isSuperAdmin)
                organizationFilter.IsDeleted = false;

            var limit = filter.PageSize;

            var offset = --filter.PageNumber * filter.PageSize;

            var query = await UnitOfWork.Repository.FindPagedAsync(predicate: PredicateBuilderFunction(organizationFilter, includeDeleted: false, governorateId: governorateId),
                pageNumber: offset,
                pageSize: limit,
                filter.OrderByValue,
                include: src => src
               //.Include(t => t.InspectionType)
               .Include(x => x.Nationality),
                cancellationToken: cancellationToken);

            var data = Mapper.Map<IEnumerable<Entities.Tracker.Organization>, IEnumerable<OrganizationDto>>(query.Result);

            return new PagingResult(filter.PageNumber, filter.PageSize, query.Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }

        public async Task<IFinalResult> GetFilterAsync(OrganizationFilter filter, CancellationToken cancellationToken = default)
        {
            // Used for internal code generation; always exclude deleted
            filter.IsDeleted = false;
            var query = await UnitOfWork.Repository.FindAsync(predicate: PredicateBuilderFunction(filter, includeDeleted: false),
                include: src => src
               //.Include(t => t.InspectionType)
               .Include(x => x.Nationality),
                cancellationToken: cancellationToken);

            var data = Mapper.Map<IEnumerable<Entities.Tracker.Organization>, IEnumerable<OrganizationDto>>(query);

            return ResponseResult.PostResult(result: data, status: HttpStatusCode.OK, exception: null,
                                              message: MessagesConstants.Success);
        }

        public async Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default)
        {
            var isSuperAdmin = IsSuperAdmin();
            var limit = filter.PageSize;

            var offset = --filter.PageNumber * filter.PageSize;

            var predicate = DropDownPredicateBuilderFunction(filter.Filter, includeDeleted: isSuperAdmin);

            var query = await UnitOfWork.Repository.FindPagedAsync(predicate: predicate, pageNumber: offset, pageSize: limit, cancellationToken: cancellationToken);

            var data = Mapper.Map<IEnumerable<Entities.Tracker.Organization>, IEnumerable<OrganizationDto>>(query.Result);

            return new PagingResult(filter.PageNumber, filter.PageSize, query.Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);

        }

        public async Task<IFinalResult> GetAllReportAsync(FilterOrgReportDTO filter, CancellationToken cancellationToken = default)
        {
            var query = await UnitOfWork.Repository.FindAsync(predicate: PredicateBuilderReportFunction(filter),
                 include: src => src
                                 .Include(x => x.Nationality),
                 cancellationToken: cancellationToken);

            var data = Mapper.Map<IEnumerable<Entities.Tracker.Organization>, IEnumerable<OrgReportDTO>>(query);

            return ResponseResult.PostResult(data, status: HttpStatusCode.OK,
                message: HttpStatusCode.OK.ToString());
        }

        public override async Task<IFinalResult> AddAsync(AddOrganizationDto model, CancellationToken cancellationToken = default)
        {
            try
            {

                string govIdForDup = GetGovernorateIdFromClaims();

                var existingForDup = model.OrganizationCategoryId != null ?
                    await UnitOfWork.Repository.FindAsync(
                    predicate: x =>
                        x.OrganizationCategoryId == model.OrganizationCategoryId &&
                        x.OrganizationType == model.OrganizationType &&
                        x.GovernorateId == govIdForDup &&
                        x.IsDeleted != true,
                    disableTracking: true,
                    cancellationToken: cancellationToken)
                    :
                    await UnitOfWork.Repository.FindAsync(
                    predicate: x =>
                        x.OrganizationType == model.OrganizationType &&
                        x.GovernorateId == govIdForDup &&
                        x.IsDeleted != true,
                    disableTracking: true,
                    cancellationToken: cancellationToken);

                if (LookupDuplicateGuard.HasFuzzyNameDuplicate(existingForDup, o => o.NameAr, o => o.NameEn, model.NameAr, model.NameEn))
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.Conflict, exception: null,
                                                     message: MessagesConstants.Existed);

                if (LookupDuplicateGuard.HasFuzzyCodeDuplicate(existingForDup, o => o.CommercialRegistrationNumber, model.CommercialRegistrationNumber))
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.Conflict, exception: null,
                                                     message: MessagesConstants.Existed);

                Entities.Tracker.Organization entity = Mapper.Map<Entities.Tracker.Organization>(model);
                entity.GovernorateId = govIdForDup;

                var data = await GetFilterAsync(new OrganizationFilter
                {
                    OrganizationType = model.OrganizationType
                }, cancellationToken);

                ICollection<OrganizationDto> dataCollection = data.Data as ICollection<OrganizationDto>;

                string segmentCode;

                if (model.OrganizationType == OrganizationType.GovernmentCompany)
                {
                    segmentCode = "Gov";
                }
                else if (model.OrganizationType == OrganizationType.OwnerCompany)
                {
                    segmentCode = "Owner";
                }
                else
                {
                    segmentCode = "Operate";
                }

                if (dataCollection.Count > 0)
                {
                    int maxNum = dataCollection
                        .Select(o =>
                        {
                            var numericPart = o.Code?.Contains('-') == true
                                ? o.Code[(o.Code.LastIndexOf('-') + 1)..]
                                : o.Code;
                            return int.TryParse(numericPart, out int n) ? n : (int?)null;
                        })
                        .Where(n => n.HasValue)
                        .Select(n => n!.Value)
                        .DefaultIfEmpty(0)
                        .Max();

                    entity.Code = segmentCode + "-" + (maxNum + 1).ToString("D3");
                }
                else
                {
                    entity.Code = segmentCode + "-001";
                }

                if (model.CommercialRegistrationAttachment != null)
                {
                    string res = await _uploaderConfiguration
                                       .UploadFile(model.CommercialRegistrationAttachment, $"Organization/{model.OrganizationType}", cancellationToken);

                    if (res != null)
                    {
                        if (UploadResponse(res) != null)
                            return UploadResponse(res);
                    }

                    entity.CommercialRegistrationAttachment = res;
                }

                SetEntityCreatedBaseProperties(entity);

                Entities.Tracker.Organization result = await UnitOfWork.Repository.AddAsync(entity, cancellationToken);

                int affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);

                if (affectedRows <= 0)
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

        public override async Task<IFinalResult> UpdateAsync(AddOrganizationDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                var govIdForDup = GetGovernorateIdFromClaims();

                var existingForDup = model.OrganizationCategoryId != null ?
                   await UnitOfWork.Repository.FindAsync(
                   predicate: x => x.Id != model.Id &&
                       x.OrganizationCategoryId == model.OrganizationCategoryId &&
                       x.OrganizationType == model.OrganizationType &&
                       x.GovernorateId == govIdForDup &&
                       x.IsDeleted != true,
                   disableTracking: true,
                   cancellationToken: cancellationToken)
                   :
                   await UnitOfWork.Repository.FindAsync(
                   predicate: x => x.Id != model.Id &&
                              x.OrganizationType == model.OrganizationType &&
                              x.GovernorateId == govIdForDup &&
                              x.IsDeleted != true,
                   disableTracking: true,
                   cancellationToken: cancellationToken);

                if (LookupDuplicateGuard.HasFuzzyNameDuplicate(existingForDup, o => o.NameAr, o => o.NameEn, model.NameAr, model.NameEn))
                    return new ResponseResult().PostResult(result: false, status: HttpStatusCode.Conflict, message: MessagesConstants.Existed);
                if (LookupDuplicateGuard.HasFuzzyCodeDuplicate(existingForDup, o => o.CommercialRegistrationNumber, model.CommercialRegistrationNumber))
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.Conflict, exception: null,
                                                     message: MessagesConstants.Existed);

                Organization entityToUpdate = await UnitOfWork.Repository.GetAsync(cancellationToken, model.Id);

                string currentCommercialRegistrationAttachment = entityToUpdate.CommercialRegistrationAttachment;

                var entity = Mapper.Map(model, entityToUpdate);
                entity.GovernorateId = GetGovernorateIdFromClaims();

                if (IsSuperAdmin())
                {
                    if (entityToUpdate.IsDeleted)
                        entity.IsDeleted = false;
                }

                if (model.CommercialRegistrationAttachment != null)
                {
                    string res = await _uploaderConfiguration
                                       .UploadFile(model.CommercialRegistrationAttachment, $"Organization/{model.OrganizationType}", cancellationToken);

                    if (res != null)
                    {
                        if (UploadResponse(res) != null)
                            return UploadResponse(res);
                    }

                    _uploaderConfiguration.DeleteFile(currentCommercialRegistrationAttachment);

                    entity.CommercialRegistrationAttachment = res;
                }
                else
                {
                    entity.CommercialRegistrationAttachment = currentCommercialRegistrationAttachment;
                }

                SetEntityModifiedBaseProperties(entity);

                UnitOfWork.Repository.Update(entityToUpdate, entity);

                int affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);

                if (affectedRows < 0)
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.BadRequest, exception: null,
                                                     message: MessagesConstants.UpdateError);

                return ResponseResult.PostResult(result: true, status: HttpStatusCode.Accepted, exception: null,
                                                 message: MessagesConstants.UpdateSuccess);
            }
            catch (Exception ex)
            {
                return ResponseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: ex,
                                                 message: MessagesConstants.UpdateError + ex.Message);
            }
        }

        public override async Task<IFinalResult> DeleteAsync(object id, CancellationToken cancellationToken = default)
        {
            try
            {
                Entities.Tracker.Organization entityToDelete = await UnitOfWork.Repository.FirstOrDefaultAsync(x => x.Id == id.ToString(), include: src => src
                                                                //.Include(x=>x.InspectionType)
                                                                .Include(x => x.Nationality),
                    cancellationToken: cancellationToken);

                _uploaderConfiguration.DeleteFile(entityToDelete.CommercialRegistrationAttachment);

                // Ensure dependent rows are removed first to avoid FK constraint conflicts
                // (FK_OrganizationStaffs_Organizations_OrganizationId).
                var staffRepo = UnitOfWork.GetRepository<Entities.Tracker.OrganizationStaff>();
                var staffToDelete = await staffRepo.FindAsync(
                    predicate: x => x.OrganizationId == entityToDelete.Id,
                    cancellationToken: cancellationToken);
                if (staffToDelete != null)
                    staffRepo.RemoveRange(staffToDelete);

                UnitOfWork.Repository.Remove(entityToDelete);

                int affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);

                if (affectedRows < 0)
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.BadRequest, exception: null,
                                                     message: MessagesConstants.DeleteError);

                return ResponseResult.PostResult(result: true, status: HttpStatusCode.Accepted, exception: null,
                                                 message: MessagesConstants.DeleteSuccess);
            }
            catch (Exception ex)
            {
                return ResponseResult.PostResult(result: false, status: HttpStatusCode.BadRequest, exception: ex,
                                                 message: MessagesConstants.DeleteError + ex.Message);
                //_logger.LogError($"{MessagesConstants.DeleteError}-{nameof(DeleteAsync)}");
                //_logger.LogError(JsonConvert.SerializeObject(e, _serializerSettings));
                throw;
            }

        }

        public async Task<byte[]> GenerateReportAsync(FilterOrgReportDTO filter, CancellationToken cancellationToken = default)
        {
            // get report file
            string fileDirPath = Assembly.GetExecutingAssembly().Location.Replace("SonoTracker.Application.dll", string.Empty);
            Console.WriteLine(string.Format(@"{0}ReportsFiles\{1}.rdlc", fileDirPath, filter.ReportName));
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

            if (filter.ReportName == "BeneficiaryOrgReport" && filter.OrganizationTypeId == OrganizationType.GovernmentCompany)
            {
                Org = await GetAllReportAsync(filter, cancellationToken);
                IEnumerable<OrgReportDTO> orgData = Org.Data as IEnumerable<OrgReportDTO> ?? throw new InvalidOperationException("No data found for the report.");
                for (int i = 0; i < orgData.ToList().Count; i++)
                {
                    orgData.ToList()[i].User = _user.Name;
                }

                //Add data source to the report
                report.DataSources.Add(new ReportDataSource() { Name = "BenificaryOrg", Value = orgData });
            }
            else if (filter.ReportName == "CompaniesOwningFloatingUnits" && filter.OrganizationTypeId == OrganizationType.OwnerCompany)
            {
                Org = await GetAllReportAsync(filter, cancellationToken);

                var orgData = Org.Data as IEnumerable<OrgReportDTO> ??
                    throw new InvalidOperationException("No data found for the report.");

                for (int i = 0; i < orgData.Count(); i++)
                {
                    PagingResult orgRes = _floatingUnitOrganizationService.GetAllPagedAsync(new BaseParam<FloatingUnitOrganizationFilter>
                    {
                        Filter = new FloatingUnitOrganizationFilter
                        {
                            OrganizationId = orgData.ElementAt(i).Id
                        }
                    }, cancellationToken).Result;
                    // Check if TouristMarinaNumber is 0 and set it to null
                    //if (orgData.ElementAt(i).TouristMarinaNumber == 0)
                    //{
                    //    orgData.ElementAt(i).TouristMarinaNumber = null;
                    //}
                }
                //Add data source to the report
                report.DataSources.Add(new ReportDataSource() { Name = "BenificaryOrg", Value = orgData });
            }
            else if (filter.ReportName == "CompaniesOperatingFloatingUnits" && filter.OrganizationTypeId == OrganizationType.OperatingCompany)
            {
                Org = await GetAllReportAsync(filter, cancellationToken);
                var orgData = Org.Data as IEnumerable<OrgReportDTO> ?? throw new InvalidOperationException("No data found for the report.");
                for (int i = 0; i < orgData.Count(); i++)
                {
                    var org = _floatingUnitOrganizationService.GetAllPagedAsync(new BaseParam<FloatingUnitOrganizationFilter>
                    {
                        Filter = new FloatingUnitOrganizationFilter
                        {
                            OrganizationId = orgData.ElementAt(i).Id
                        }
                    }, cancellationToken).Result;
                }
                //Add data source to the report
                report.DataSources.Add(new ReportDataSource() { Name = "BenificaryOrg", Value = orgData });
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

        static Expression<Func<Entities.Tracker.Organization, bool>> PredicateBuilderFunction(OrganizationFilter filter, bool includeDeleted, string governorateId = null)
        {
            var predicate = includeDeleted
                ? PredicateBuilder.New<Entities.Tracker.Organization>(true)
                : PredicateBuilder.New<Entities.Tracker.Organization>(x => x.IsDeleted == filter.IsDeleted);
            if (!string.IsNullOrWhiteSpace(filter?.NameAr))
            {
                predicate = predicate.And(x => x.NameAr.Contains(filter.NameAr));
            }
            if (!string.IsNullOrWhiteSpace(filter?.Code))
            {
                predicate = predicate.And(x => x.Code.Contains(filter.Code));
            }

            if (!string.IsNullOrWhiteSpace(filter?.NameEn))
            {
                predicate = predicate.And(x => x.NameEn.Contains(filter.NameEn));
            }
            if (!string.IsNullOrWhiteSpace(filter?.OrganizationCategoryId))
            {
                predicate = predicate.And(x => x.OrganizationCategoryId.Equals(filter.OrganizationCategoryId));
            }
            if (filter.OrganizationType.HasValue)
            {
                predicate = predicate.And(x => x.OrganizationType == filter.OrganizationType.Value);
            }

            if (!string.IsNullOrWhiteSpace(governorateId))
            {
                predicate = predicate.And(x => x.GovernorateId == governorateId);
            }
            //if (filter.AppliedOn.HasValue)
            //{
            //    predicate = predicate.And(x => x.AppliedOn == filter.AppliedOn.Value);
            //}

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

        static Expression<Func<Entities.Tracker.Organization, bool>> DropDownPredicateBuilderFunction(SearchCriteriaFilter filter, bool includeDeleted)
        {
            var predicate = includeDeleted
                ? PredicateBuilder.New<Entities.Tracker.Organization>(true)
                : PredicateBuilder.New<Entities.Tracker.Organization>(x => x.IsDeleted != true);
            if (!string.IsNullOrWhiteSpace(filter?.SearchCriteria))
            {
                predicate = predicate.And(b => b.NameAr.Contains(filter.SearchCriteria));
                predicate = predicate.Or(b => b.NameEn.Contains(filter.SearchCriteria));
                predicate = predicate.Or(b => b.Code.Contains(filter.SearchCriteria));
            }
            return predicate;
        }

        static Expression<Func<Entities.Tracker.Organization, bool>> PredicateBuilderReportFunction(FilterOrgReportDTO filter)
        {
            var predicate = PredicateBuilder.New<Entities.Tracker.Organization>(x => x.IsDeleted != true);

            if (filter.OrganizationIds.Length > 0)
            {
                predicate = predicate.And(e => filter.OrganizationIds.Any(id => id == e.Id));
            }
            if (!string.IsNullOrWhiteSpace(filter?.NameAr))
            {
                predicate = predicate.And(x => x.NameAr.Contains(filter.NameAr));
            }
            if (!string.IsNullOrWhiteSpace(filter?.NameEn))
            {
                predicate = predicate.And(x => x.NameEn.Contains(filter.NameEn));
            }
            if (filter.OrganizationTypeId.HasValue)
            {
                predicate = predicate.And(x => x.OrganizationType == filter.OrganizationTypeId.Value);
            }


            return predicate;
        }

        private IFinalResult UploadResponse(string res)
        {
            if (res == "Size")
            {
                var message = "File Size Larger than 5 Mega Bytes";
                return ResponseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, message: message);
            }
            else if (res == "Type")
            {
                var message = "File type not allowed.";
                return ResponseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, message: message);
            }
            else
            {
                return null;
            }
        }
    }
}

