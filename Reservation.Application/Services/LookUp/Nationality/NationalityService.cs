using LinqKit;
using SonoTracker.Application.Services.Base;
using SonoTracker.Common.Helpers;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Lookup.City;
using SonoTracker.Common.DTO.Lookup.Nationality;
using SonoTracker.Common.DTO.Lookup.Nationality.Parameters;
using SonoTracker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoTracker.Application.Services.Lookup.Nationality
{
    public class NationalityService(IServiceBaseParameter<Domain.Entities.Lookups.Nationality> businessBaseParameter) : BaseService<Domain.Entities.Lookups.Nationality, AddNationalityDto, EditNationalityDto, NationalityDto, string, string>(businessBaseParameter), INationalityService
    {
        public override async Task<IFinalResult> GetAllAsync(bool disableTracking = false, Expression<Func<Domain.Entities.Lookups.Nationality, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            // Retrieve all entities
            var entity = await UnitOfWork.Repository.GetAllAsync(disableTracking: disableTracking, cancellationToken: cancellationToken);

            // Filter out deleted records (except for SuperAdmin)
            var filteredEntities = IsSuperAdmin()
                ? entity
                : entity.Where(e => !e.IsDeleted);

            var mapped = Mapper.Map<IEnumerable<Domain.Entities.Lookups.Nationality>, IEnumerable<NationalityDto>>(filteredEntities);

            return ResponseResult.PostResult(mapped, status: HttpStatusCode.OK,
                message: HttpStatusCode.OK.ToString());
        }
        public async Task<PagingResult> GetAllPagedAsync(BaseParam<NationalityFilter> filter, CancellationToken cancellationToken = default)
        {
            var isSuperAdmin = IsSuperAdmin();
            var nationalityFilter = filter?.Filter ?? new NationalityFilter();

            if (!isSuperAdmin)
                nationalityFilter.IsDeleted = false;

            var limit = filter.PageSize;

            var offset = --filter.PageNumber * filter.PageSize;

            var query = await UnitOfWork.Repository.FindPagedAsync
                (predicate: PredicateBuilderFunction(nationalityFilter),
                pageNumber: offset,
                pageSize: limit, filter.OrderByValue,
                cancellationToken: cancellationToken);

            var filteredItem2 = isSuperAdmin
                ? query.Item2
                : query.Item2.Where(x => x.IsDeleted != true);

            var data = Mapper.Map<IEnumerable<Domain.Entities.Lookups.Nationality>, IEnumerable<NationalityDto>>(filteredItem2);

            return new PagingResult(filter.PageNumber, filter.PageSize, query.Item1, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }
        public async Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default)
        {
            var limit = filter.PageSize;

            var offset = --filter.PageNumber * filter.PageSize;

            var predicate = DropDownPredicateBuilderFunction(filter.Filter);

            var query = await UnitOfWork.Repository.FindPagedAsync(predicate: predicate, pageNumber: offset, pageSize: limit, cancellationToken: cancellationToken);

            var data = Mapper.Map<IEnumerable<Domain.Entities.Lookups.Nationality>, IEnumerable<NationalityDto>>(query.Item2.Where(x => x.IsDeleted != true));

            return new PagingResult(filter.PageNumber, filter.PageSize, query.Item1, data, status: HttpStatusCode.OK, MessagesConstants.Success);

        }
        static Expression<Func<Domain.Entities.Lookups.Nationality, bool>> PredicateBuilderFunction(NationalityFilter filter)
        {
            var predicate = PredicateBuilder.New<Domain.Entities.Lookups.Nationality>(x => x.IsDeleted == filter.IsDeleted);
            if (!string.IsNullOrWhiteSpace(filter.NameAr))
            {
                predicate = predicate.And(x => x.NameAr.Contains(filter.NameAr));
            }
            if (!string.IsNullOrWhiteSpace(filter.NameEn))
            {
                predicate = predicate.And(x => x.NameEn.Contains(filter.NameEn));
            }

            return predicate;
        }
        static Expression<Func<Domain.Entities.Lookups.Nationality, bool>> DropDownPredicateBuilderFunction(SearchCriteriaFilter filter)
        {
            var predicate = PredicateBuilder.New<Domain.Entities.Lookups.Nationality>(true);
            if (!string.IsNullOrWhiteSpace(filter?.SearchCriteria))
            {
                predicate = predicate.And(b => b.NameAr.Contains(filter.SearchCriteria));
                predicate = predicate.Or(b => b.NameEn.Contains(filter.SearchCriteria));
            }
            return predicate;
        }
        public override async Task<IFinalResult> AddAsync(AddNationalityDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                var existingForDup = await UnitOfWork.Repository.FindAsync(disableTracking: true, cancellationToken: cancellationToken);

                if (LookupDuplicateGuard.HasFuzzyNameDuplicate(existingForDup, x => x.NameAr, x => x.NameEn, model.NameAr, model.NameEn))
                    return new ResponseResult().PostResult(result: false, status: HttpStatusCode.Conflict,
                                                message: MessagesConstants.Existed);

                var entity = Mapper.Map<AddNationalityDto, Entities.Lookups.Nationality>(model);

                var data = await GetAllAsync(cancellationToken: cancellationToken);

                var dataCollection = data.Data as ICollection<NationalityDto>;

                if (dataCollection?.Count > 0)
                {
                    if (int.TryParse(dataCollection.OrderByDescending(o => o.Code).FirstOrDefault().Code.
                        AsSpan(dataCollection.OrderByDescending(o => o.Code).FirstOrDefault().Code.Length - 1),
                        out int num))
                    {
                        int newCode = ++num;
                        entity.Code = newCode.ToString("D4");
                    }
                }
                else
                {
                    entity.Code = "0001";
                }

                SetEntityCreatedBaseProperties(entity);
                await UnitOfWork.Repository.AddAsync(entity, cancellationToken);
                var affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);

                if (affectedRows <= 0) return ResponseResult.PostResult(false, HttpStatusCode.BadRequest, null, MessagesConstants.AddError);

                return ResponseResult.PostResult(result: entity.Id, HttpStatusCode.Created, null, MessagesConstants.AddSuccess);
            }
            catch (Exception ex)
            {
                return ResponseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: ex,
                    message: MessagesConstants.AddError + ex.Message);
            }

        }

        public override async Task<IFinalResult> UpdateAsync(AddNationalityDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                var existingForDup = await UnitOfWork.Repository.FindAsync(
                    predicate: x => x.Id != model.Id,
                    disableTracking: true,
                    cancellationToken: cancellationToken);

                if (LookupDuplicateGuard.HasFuzzyNameDuplicate(existingForDup, x => x.NameAr, x => x.NameEn, model.NameAr, model.NameEn))
                    return new ResponseResult().PostResult(result: false, status: HttpStatusCode.Conflict, message: MessagesConstants.Existed);

                Entities.Lookups.Nationality entityToUpdate = await UnitOfWork.Repository.GetAsync(cancellationToken, model.Id);

                var entity = Mapper.Map(model, entityToUpdate);

                if (IsSuperAdmin())
                {
                    if (entityToUpdate.IsDeleted)
                        entity.IsDeleted = false;
                }
                SetEntityModifiedBaseProperties(entity);
                UnitOfWork.Repository.Update(entityToUpdate, entity);

                int affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);

                if (affectedRows < 0) return ResponseResult.PostResult(false, HttpStatusCode.BadRequest, null, MessagesConstants.UpdateError);

                return ResponseResult.PostResult(true, HttpStatusCode.OK, null, MessagesConstants.UpdateSuccess);
            }
            catch (Exception ex)
            {
                return ResponseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: ex,
                    message: MessagesConstants.UpdateError + ex.Message);
            }
        }

    }
}
