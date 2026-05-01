using LinqKit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SonoTracker.Application.Services.Base;
using SonoTracker.Application.Services.Tracker.FloatingUnits;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Tracker.Maintenance.Parameters;
using SonoTracker.Common.DTO.Tracker.Maintenance;
using SonoTracker.Common.Helpers.MediaUploader;
using SonoTracker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using SonoTracker.Common.DTO.Tracker.FloatingUnit;
using SonoTracker.Domain.Entities.Tracker;
using SonoTracker.Common.DTO.Tracker.Organization;
using SonoTracker.Application.Services.Tracker.Organizations;
using SonoTracker.Common.DTO.Tracker.TouristMarinaLicenseApplication.Parameters;
using SonoTracker.Common.DTO.Tracker.TouristMarinaLicenseApplication;


namespace SonoTracker.Application.Services.Tracker.TouristMarinaLicenseApplication
{
    public class TouristMarinaLicenseApplicationService : BaseService<Entities.Tracker.TouristMarinaLicenseApplication, AddTouristMarinaLicenseApplicationDto, EditTouristMarinaLicenseApplicationDto, TouristMarinaLicenseApplicationDto, string, string>, ITouristMarinaLicenseApplicationService
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IHttpContextAccessor _request;
        private readonly UploaderConfiguration _uploaderConfiguration;
        private readonly IOrganizationService _organizationService;
        public TouristMarinaLicenseApplicationService(IServiceBaseParameter<Entities.Tracker.TouristMarinaLicenseApplication> businessBaseParameter,
            IWebHostEnvironment hostingEnvironment, IHttpContextAccessor request, IOrganizationService organizationService) : base(businessBaseParameter)
        {
            _hostingEnvironment = hostingEnvironment;
            _request = request;
            _organizationService = organizationService;
            _uploaderConfiguration = new UploaderConfiguration(_hostingEnvironment, _request);
        }
        public override async Task<IFinalResult> GetByIdForEditAsync(object id, CancellationToken cancellationToken = default)
        {
            var entity = await UnitOfWork.Repository.FirstOrDefaultAsync(x => x.Id == id.ToString(),
                               include: src => src
                              .Include(t => t.FromOrganization)
                              .Include(t => t.ToOrganization), cancellationToken: cancellationToken);

            var mapped = Mapper.Map<Entities.Tracker.TouristMarinaLicenseApplication, EditTouristMarinaLicenseApplicationDto>(entity);

            return ResponseResult.PostResult(mapped, HttpStatusCode.OK);
        }

        public override async Task<IFinalResult> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            var entity = await UnitOfWork.Repository.FirstOrDefaultAsync(x => x.Id == id.ToString(),
                               include: src => src
                              .Include(t => t.FromOrganization)
                              .Include(t => t.ToOrganization), cancellationToken: cancellationToken);

            var mapped = Mapper.Map<Entities.Tracker.TouristMarinaLicenseApplication, TouristMarinaLicenseApplicationDto>(entity);

            return ResponseResult.PostResult(result: mapped, status: HttpStatusCode.OK, exception: null,
                                             message: HttpStatusCode.OK.ToString());
        }
        public override async Task<IFinalResult> GetAllAsync(bool disableTracking = false, Expression<Func<Entities.Tracker.TouristMarinaLicenseApplication, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            var entity = await UnitOfWork.Repository.GetAllAsync(include: src => src
                .Include(t => t.FromOrganization)
                .Include(t => t.ToOrganization), cancellationToken: cancellationToken);

            var governorateId = IsSuperAdmin() ? null : GetGovernorateIdFromClaims();
            var filteredEntities = IsSuperAdmin()
                ? entity
                : entity.Where(e => !e.IsDeleted && (string.IsNullOrWhiteSpace(governorateId) || e.GovernorateId == governorateId));

            var mapped = Mapper.Map<IEnumerable<Entities.Tracker.TouristMarinaLicenseApplication>, IEnumerable<TouristMarinaLicenseApplicationDto>>(filteredEntities);

            return ResponseResult.PostResult(mapped, status: HttpStatusCode.OK,
                message: HttpStatusCode.OK.ToString());
        }

        public async Task<PagingResult> GetAllPagedAsync(BaseParam<TouristMarinaLicenseApplicationFilter> filter, CancellationToken cancellationToken = default)
        {
            var isSuperAdmin = IsSuperAdmin();
            var licenseFilter = filter?.Filter ?? new TouristMarinaLicenseApplicationFilter();
            var governorateId = isSuperAdmin ? null : GetGovernorateIdFromClaims();

            if (!isSuperAdmin)
                licenseFilter.IsDeleted = false;

            var limit = filter.PageSize;

            var offset = --filter.PageNumber * filter.PageSize;

            var query = await UnitOfWork.Repository.FindPagedAsync(predicate: PredicateBuilderFunction(licenseFilter, governorateId), pageNumber: offset, pageSize: limit, filter.OrderByValue, cancellationToken: cancellationToken);

            var data = Mapper.Map<IEnumerable<Entities.Tracker.TouristMarinaLicenseApplication>, IEnumerable<TouristMarinaLicenseApplicationDto>>(query.Item2);

            return new PagingResult(filter.PageNumber, filter.PageSize, query.Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }
        public async Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default)
        {
            var limit = filter.PageSize;

            var offset = --filter.PageNumber * filter.PageSize;

            var isSuperAdmin = IsSuperAdmin();
            var predicate = DropDownPredicateBuilderFunction(filter.Filter);

            var query = await UnitOfWork.Repository.FindPagedAsync(predicate: predicate, pageNumber: offset, pageSize: limit, cancellationToken: cancellationToken);

            var items = isSuperAdmin ? query.Result : query.Result.Where(x => x.IsDeleted != true);
            var data = Mapper.Map<IEnumerable<Entities.Tracker.TouristMarinaLicenseApplication>, IEnumerable<TouristMarinaLicenseApplicationDto>>(items);

            return new PagingResult(filter.PageNumber, filter.PageSize, query.Item1, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }


        static Expression<Func<Entities.Tracker.TouristMarinaLicenseApplication, bool>> PredicateBuilderFunction(TouristMarinaLicenseApplicationFilter filter, string governorateId = null)
        {
            var predicate = PredicateBuilder.New<Entities.Tracker.TouristMarinaLicenseApplication>(x => x.IsDeleted == filter.IsDeleted);

            if (!string.IsNullOrWhiteSpace(filter.LicenseNumber))
            {
                predicate = predicate.And(x => x.LicenseNumber.Equals(filter.LicenseNumber));
            }
            if (filter.StartDate.HasValue)
            {
                predicate = predicate.And(x => x.LicenseDate > filter.StartDate);
            }
            if (filter.EndDate.HasValue)
            {
                predicate = predicate.And(x => x.LicenseDate < filter.EndDate);
            }
            if (!string.IsNullOrEmpty(filter.FromOrganizationId))
            {
                predicate = predicate.And(x => x.FromOrganizationId == filter.FromOrganizationId);
            }
            if (!string.IsNullOrEmpty(filter.ToOrganizationId))
            {
                predicate = predicate.And(x => x.ToOrganizationId == filter.ToOrganizationId);
            }
            if (filter.Status.HasValue)
            {
                predicate = predicate.And(x => x.Status == filter.Status);
            }


            if (!string.IsNullOrWhiteSpace(governorateId))
            {
                predicate = predicate.And(x => x.GovernorateId == governorateId);
            }

            return predicate;
        }
        static Expression<Func<Entities.Tracker.TouristMarinaLicenseApplication, bool>> DropDownPredicateBuilderFunction(SearchCriteriaFilter filter)
        {
            var predicate = PredicateBuilder.New<Entities.Tracker.TouristMarinaLicenseApplication>(true);
            if (!string.IsNullOrWhiteSpace(filter?.SearchCriteria))
            {
                //predicate = predicate.And(b => b.Name.Contains(filter.SearchCriteria));
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
        public async Task<IFinalResult> GetAllFilterAsync(TouristMarinaLicenseApplicationFilter filter, CancellationToken cancellationToken = default)
        {
            var entity = await UnitOfWork.Repository.FindAsync(predicate: PredicateBuilderFunction(filter), cancellationToken: cancellationToken);

            var data = Mapper.Map<IEnumerable<Entities.Tracker.TouristMarinaLicenseApplication>, IEnumerable<TouristMarinaLicenseApplicationDto>>(entity.Where(x => x.IsDeleted != true));

            return ResponseResult.PostResult(data, status: HttpStatusCode.OK,
                message: HttpStatusCode.OK.ToString());
        }
        public override async Task<IFinalResult> AddAsync(AddTouristMarinaLicenseApplicationDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var mapped = Mapper.Map<Entities.Tracker.TouristMarinaLicenseApplication>(dto);
                mapped.GovernorateId = GetGovernorateIdFromClaims();

                SetEntityCreatedBaseProperties(mapped);

                IFinalResult orgnaization = await _organizationService.GetByIdAsync(dto.FromOrganizationId, cancellationToken);

                string organizationCode;

                if (orgnaization.Data is OrganizationDto orgnaizationDto)
                {
                    organizationCode = orgnaizationDto.Code;

                    TouristMarinaLicenseApplicationFilter filter = new()
                    {
                        FromOrganizationId = dto.FromOrganizationId,
                        IsDeleted = false
                    };

                    // Check if the trip information already exists for the given floating unit id
                    var exist = await GetAllFilterAsync(filter, cancellationToken);
                    // Fix: Explicitly cast exist.Data to a collection type to access the Count property.
                    var existDataCollection = exist.Data as ICollection<TouristMarinaLicenseApplicationDto>;

                    if (existDataCollection?.Count > 0)
                    {
                        // Handle existing trip information logic here (if needed)
                        var lastLicenseApplication = existDataCollection.LastOrDefault();
                        if (lastLicenseApplication != null && lastLicenseApplication.LicenseNumber.StartsWith(organizationCode))
                        {
                            // Extract the numeric part and increment it
                            var numericPart = lastLicenseApplication.LicenseNumber[organizationCode.Length..];
                            if (int.TryParse(numericPart, out int number))
                            {
                                number++;
                                mapped.LicenseNumber = organizationCode + number.ToString("D2"); // Ensure 2 digits
                            }
                        }
                    }
                    else
                    {
                        mapped.LicenseNumber = organizationCode + "01";
                    }
                }

                if (dto.Insurance != null)
                {
                    string res = await _uploaderConfiguration
                                       .UploadFile(dto.Insurance, "LicenseApplication/Insurance", cancellationToken);

                    if (res != null)
                    {
                        if (UploadResponse(res) != null)
                            return UploadResponse(res);
                    }

                    mapped.Insurance = res;
                }
                if (dto.CommercialRegister != null)
                {
                    string res = await _uploaderConfiguration
                                       .UploadFile(dto.CommercialRegister, "LicenseApplication/CommercialRegister", cancellationToken);

                    if (res != null)
                    {
                        if (UploadResponse(res) != null)
                            return UploadResponse(res);
                    }

                    mapped.CommercialRegister = res;
                }
                if (dto.Taxes != null)
                {
                    string res = await _uploaderConfiguration
                                       .UploadFile(dto.Taxes, "LicenseApplication/Taxes", cancellationToken);

                    if (res != null)
                    {
                        if (UploadResponse(res) != null)
                            return UploadResponse(res);
                    }

                    mapped.Taxes = res;
                }
                if (dto.CivilProtection != null)
                {
                    string res = await _uploaderConfiguration
                                       .UploadFile(dto.CivilProtection, "LicenseApplication/CivilProtection", cancellationToken);

                    if (res != null)
                    {
                        if (UploadResponse(res) != null)
                            return UploadResponse(res);
                    }

                    mapped.CivilProtection = res;
                }
                if (dto.Irrigation != null)
                {
                    string res = await _uploaderConfiguration
                                       .UploadFile(dto.Irrigation, "LicenseApplication/Irragtion", cancellationToken);

                    if (res != null)
                    {
                        if (UploadResponse(res) != null)
                            return UploadResponse(res);
                    }

                    mapped.Irrigation = res;
                }
                if (dto.StateProperty != null)
                {
                    string res = await _uploaderConfiguration
                                       .UploadFile(dto.StateProperty, "LicenseApplication/StateProperty", cancellationToken);

                    if (res != null)
                    {
                        if (UploadResponse(res) != null)
                            return UploadResponse(res);
                    }

                    mapped.StateProperty = res;
                }
                if (dto.Other != null)
                {
                    string res = await _uploaderConfiguration
                                       .UploadFile(dto.Other, "LicenseApplication/Other", cancellationToken);

                    if (res != null)
                    {
                        if (UploadResponse(res) != null)
                            return UploadResponse(res);
                    }

                    mapped.OtherAttach = res;
                }

                mapped.IsDeleted = false;

                UnitOfWork.Repository.Add(mapped);

                await UnitOfWork.SaveChangesAsync(cancellationToken);

                return ResponseResult.PostResult(result: mapped.Id, status: HttpStatusCode.Created, exception: null,
                                                 message: HttpStatusCode.Created.ToString());
            }
            catch (Exception ex)
            {
                return ResponseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: ex,
                                      message: HttpStatusCode.BadRequest.ToString());
            }
        }

        public override async Task<IFinalResult> UpdateAsync(AddTouristMarinaLicenseApplicationDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                Entities.Tracker.TouristMarinaLicenseApplication entityToUpdate = await UnitOfWork.Repository.GetAsync(cancellationToken, dto.Id);

                string currentInsurance = entityToUpdate.Insurance;
                string currentCommercialRegister = entityToUpdate.CommercialRegister;
                string currentTaxes = entityToUpdate.Taxes;
                string currentCivilProtection = entityToUpdate.CivilProtection;
                string currentIrrigation = entityToUpdate.Irrigation;
                string currentStateProperty = entityToUpdate.StateProperty;
                string currentOtherAttach = entityToUpdate.OtherAttach;

                Entities.Tracker.TouristMarinaLicenseApplication newEntity = Mapper.Map(dto, entityToUpdate);
                newEntity.GovernorateId = GetGovernorateIdFromClaims();

                SetEntityModifiedBaseProperties(newEntity);

                if (IsSuperAdmin())
                {
                    if (entityToUpdate.IsDeleted)
                        newEntity.IsDeleted = false;
                }

                if (dto.Insurance != null)
                {
                    string res = await _uploaderConfiguration
                                       .UploadFile(dto.Insurance, "LicenseApplication/Insurance", cancellationToken);

                    if (res != null)
                    {
                        if (UploadResponse(res) != null)
                            return UploadResponse(res);
                    }

                    _uploaderConfiguration.DeleteFile(entityToUpdate.Insurance);

                    newEntity.Insurance = res;
                }
                else
                {
                    newEntity.Insurance = currentInsurance;
                }

                if (dto.CommercialRegister != null)
                {
                    string res = await _uploaderConfiguration
                                       .UploadFile(dto.CommercialRegister, "LicenseApplication/CommercialRegister", cancellationToken);

                    if (res != null)
                    {
                        if (UploadResponse(res) != null)
                            return UploadResponse(res);
                    }

                    _uploaderConfiguration.DeleteFile(entityToUpdate.CommercialRegister);

                    newEntity.CommercialRegister = res;
                }
                else
                {
                    newEntity.CommercialRegister = currentCommercialRegister;
                }

                if (dto.Taxes != null)
                {
                    string res = await _uploaderConfiguration
                                       .UploadFile(dto.Taxes, "LicenseApplication/Taxes", cancellationToken);

                    if (res != null)
                    {
                        if (UploadResponse(res) != null)
                            return UploadResponse(res);
                    }

                    _uploaderConfiguration.DeleteFile(entityToUpdate.Taxes);

                    newEntity.Taxes = res;
                }
                else
                {
                    newEntity.Taxes = currentTaxes;
                }

                if (dto.CivilProtection != null)
                {
                    string res = await _uploaderConfiguration
                                       .UploadFile(dto.CommercialRegister, "LicenseApplication/CivilProtection", cancellationToken);

                    if (res != null)
                    {
                        if (UploadResponse(res) != null)
                            return UploadResponse(res);
                    }

                    _uploaderConfiguration.DeleteFile(entityToUpdate.CivilProtection);

                    newEntity.CivilProtection = res;
                }
                else
                {
                    newEntity.CivilProtection = currentCivilProtection;
                }

                if (dto.Irrigation != null)
                {
                    string res = await _uploaderConfiguration
                                       .UploadFile(dto.Irrigation, "LicenseApplication/Irrigation", cancellationToken);

                    if (res != null)
                    {
                        if (UploadResponse(res) != null)
                            return UploadResponse(res);
                    }

                    _uploaderConfiguration.DeleteFile(entityToUpdate.Irrigation);

                    newEntity.Irrigation = res;
                }
                else
                {
                    newEntity.Irrigation = currentIrrigation;
                }

                if (dto.StateProperty != null)
                {
                    string res = await _uploaderConfiguration
                                       .UploadFile(dto.StateProperty, "LicenseApplication/StateProperty", cancellationToken);

                    if (res != null)
                    {
                        if (UploadResponse(res) != null)
                            return UploadResponse(res);
                    }

                    _uploaderConfiguration.DeleteFile(entityToUpdate.StateProperty);

                    newEntity.StateProperty = res;
                }
                else
                {
                    newEntity.StateProperty = currentStateProperty;
                }

                if (dto.Other != null)
                {
                    string res = await _uploaderConfiguration
                                       .UploadFile(dto.CommercialRegister, "LicenseApplication/Other", cancellationToken);

                    if (res != null)
                    {
                        if (UploadResponse(res) != null)
                            return UploadResponse(res);
                    }

                    _uploaderConfiguration.DeleteFile(entityToUpdate.OtherAttach);

                    newEntity.OtherAttach = res;
                }
                else
                {
                    newEntity.OtherAttach = currentOtherAttach;
                }

                UnitOfWork.Repository.Update(entityToUpdate, newEntity);

                var affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);

                if (affectedRows > 0)
                {
                    Result = ResponseResult.PostResult(result: true, status: HttpStatusCode.Accepted, exception: null,
                                                       message: MessagesConstants.UpdateSuccess);
                }

                return Result;
            }
            catch (Exception ex)
            {
                return ResponseResult.PostResult(result: false, status: HttpStatusCode.Accepted, exception: ex,
                                                 message: MessagesConstants.UpdateSuccess);
                //_logger.LogError($"{MessagesConstants.UpdateError}-{nameof(UpdateAsync)}");
                //_logger.LogError(JsonConvert.SerializeObject(e, _serializerSettings));
                throw;
            }

        }

        public override async Task<IFinalResult> DeleteAsync(object id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entityToDelete = await UnitOfWork.Repository.GetAsync(cancellationToken, id);

                // Remove Uploaded Files
                _uploaderConfiguration.DeleteFile(entityToDelete.Insurance);
                _uploaderConfiguration.DeleteFile(entityToDelete.CommercialRegister);
                _uploaderConfiguration.DeleteFile(entityToDelete.Taxes);
                _uploaderConfiguration.DeleteFile(entityToDelete.CivilProtection);
                _uploaderConfiguration.DeleteFile(entityToDelete.Irrigation);
                _uploaderConfiguration.DeleteFile(entityToDelete.StateProperty);
                _uploaderConfiguration.DeleteFile(entityToDelete.OtherAttach);

                UnitOfWork.Repository.Remove(entityToDelete);

                var affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);

                if (affectedRows > 0)
                {
                    Result = ResponseResult.PostResult(result: true, status: HttpStatusCode.Accepted,
                        message: MessagesConstants.DeleteSuccess);
                }

                return Result;
            }
            catch (Exception e)
            {
                //_logger.LogError($"{MessagesConstants.DeleteError}-{nameof(DeleteAsync)}");
                //_logger.LogError(JsonConvert.SerializeObject(e, _serializerSettings));
                throw;
            }

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
