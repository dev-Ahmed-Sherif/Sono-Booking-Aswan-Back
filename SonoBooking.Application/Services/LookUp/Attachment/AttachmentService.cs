using LinqKit;
using SonoBooking.Application.Services.Base;
using SonoBooking.Common.DTO.Lookup.Attachment;
using SonoBooking.Common.DTO.Lookup.Attachment.Parameters;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoTracker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AttachmentEntity = SonoBooking.Domain.Entities.Lookups.Attachment;
using SonoBooking.Domain;

namespace SonoBooking.Application.Services.LookUp.Attachment
{
    public class AttachService(IServiceBaseParameter<AttachmentEntity> businessBaseParameter)
        : BaseService<AttachmentEntity, AddAttachmentDto, EditAttachmentDto, AttachmentDto, string, string>(businessBaseParameter), IAttachService
    {
        public override async Task<IFinalResult> GetAllAsync(bool disableTracking = false, Expression<Func<AttachmentEntity, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            var isSuperAdmin = IsSuperAdmin();

            IEnumerable<AttachmentEntity> entities = await UnitOfWork.Repository.FindAsync(
                predicate: predicate,
                disableTracking: disableTracking,
                cancellationToken: cancellationToken);

            IEnumerable<AttachmentEntity> filteredEntities = isSuperAdmin
                ? (entities ?? [])
                : (entities?.Where(x => !x.IsDeleted) ?? []);

            var mapped = Mapper.Map<IEnumerable<AttachmentEntity>, IEnumerable<AttachmentDto>>(filteredEntities);

            return ResponseResult.PostResult(result: mapped, status: HttpStatusCode.OK, exception: null,
                message: HttpStatusCode.OK.ToString());
        }

        public async Task<PagingResult> GetAllPagedAsync(BaseParam<AttachmentFilter> filter, CancellationToken cancellationToken = default)
        {
            var limit = filter.PageSize;
            var offset = --filter.PageNumber * filter.PageSize;
            var attachmentFilter = filter?.Filter ?? new AttachmentFilter();

            (int Count, IEnumerable<AttachmentEntity> Result) =
                await UnitOfWork.Repository.FindPagedAsync(
                    predicate: PredicateBuilderFunction(attachmentFilter),
                    pageNumber: offset,
                    pageSize: limit,
                    filter.OrderByValue,
                    cancellationToken: cancellationToken);

            var data = Mapper.Map<IEnumerable<AttachmentEntity>, IEnumerable<AttachmentDto>>(Result ?? []);
            return new PagingResult(filter.PageNumber, filter.PageSize, Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }

        public async Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default)
        {
            var limit = filter.PageSize;
            var offset = --filter.PageNumber * filter.PageSize;
            var predicate = DropDownPredicateBuilderFunction(filter.Filter);

            (int Count, IEnumerable<AttachmentEntity> Result) query =
                await UnitOfWork.Repository.FindPagedAsync(predicate: predicate, pageNumber: offset, pageSize: limit, cancellationToken: cancellationToken);

            var data = Mapper.Map<IEnumerable<AttachmentEntity>, IEnumerable<AttachmentDto>>(query.Result ?? []);
            return new PagingResult(filter.PageNumber, filter.PageSize, query.Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }

        static Expression<Func<AttachmentEntity, bool>> PredicateBuilderFunction(AttachmentFilter filter)
        {
            var predicate = PredicateBuilder.New<AttachmentEntity>(x => x.IsDeleted == filter.IsDeleted);
            //if (!string.IsNullOrWhiteSpace(filter.FileName))
            //    predicate = predicate.And(x => x.FileName.Contains(filter.FileName));

            return predicate;
        }

        static Expression<Func<AttachmentEntity, bool>> DropDownPredicateBuilderFunction(SearchCriteriaFilter filter)
        {
            var predicate = PredicateBuilder.New<AttachmentEntity>(true);
            if (!string.IsNullOrWhiteSpace(filter?.SearchCriteria))
                predicate = predicate.And(x => x.FileName.Contains(filter.SearchCriteria));

            return predicate;
        }
    }
}
