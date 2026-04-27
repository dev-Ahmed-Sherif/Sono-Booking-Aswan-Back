using LinqKit;
using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SonoTracker.Application.Services.Base;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Tracker.InspectionAttach;
using SonoTracker.Common.DTO.Tracker.InspectionAttach.Parameters;
using SonoTracker.Common.Helpers.MediaUploader;
using SonoTracker.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoTracker.Application.Services.Tracker.InspectionAttach
{
    public class InspectionAttachService : BaseService<Entities.Tracker.InspectionAttachment, AddInspectionAttachDto, EditInspectionAttachDto, InspectionAttachDto, string, string>, IInspectionAttachService
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IHttpContextAccessor _request;
        private readonly UploaderConfiguration _uploaderConfiguration;

        public InspectionAttachService(
            IServiceBaseParameter<Entities.Tracker.InspectionAttachment> businessBaseParameter,
            IWebHostEnvironment hostingEnvironment,
            IHttpContextAccessor request) : base(businessBaseParameter)
        {
            _hostingEnvironment = hostingEnvironment;
            _request = request;
            _uploaderConfiguration = new UploaderConfiguration(_hostingEnvironment, _request);
        }

        public override async Task<IFinalResult> GetByIdForEditAsync(object id, CancellationToken cancellationToken = default)
        {
            var entity = await UnitOfWork.Repository.FirstOrDefaultAsync(x => x.Id == id.ToString(),
                           include: src => src.Include(t => t.Attachment), cancellationToken: cancellationToken);

            var mapped = Mapper.Map<Entities.Tracker.InspectionAttachment, EditInspectionAttachDto>(entity);
            return ResponseResult.PostResult(mapped, HttpStatusCode.OK);
        }

        public override async Task<IFinalResult> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            var entity = await UnitOfWork.Repository.FirstOrDefaultAsync(x => x.Id == id.ToString(),
                include: src => src.Include(t => t.Attachment), cancellationToken: cancellationToken);

            var mapped = Mapper.Map<Entities.Tracker.InspectionAttachment, InspectionAttachDto>(entity);
            return ResponseResult.PostResult(mapped, HttpStatusCode.OK);
        }

        public override async Task<IFinalResult> GetAllAsync(bool disableTracking = false, Expression<Func<Entities.Tracker.InspectionAttachment, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            var entity = await UnitOfWork.Repository.GetAllAsync(
                include: src => src.Include(t => t.Attachment), cancellationToken: cancellationToken);

            var filteredEntities = entity.Where(e => !e.IsDeleted);
            IEnumerable<InspectionAttachDto> mapped = Mapper.Map<IEnumerable<Entities.Tracker.InspectionAttachment>, IEnumerable<InspectionAttachDto>>(filteredEntities);

            return ResponseResult.PostResult(mapped, status: HttpStatusCode.OK, message: HttpStatusCode.OK.ToString());
        }

        public async Task<PagingResult> GetAllPagedAsync(BaseParam<InspectionAttachFilter> filter, CancellationToken cancellationToken = default)
        {
            var limit = filter.PageSize;
            var offset = --filter.PageNumber * filter.PageSize;

            var query = await UnitOfWork.Repository.FindPagedAsync(
                predicate: PredicateBuilderFunction(filter.Filter),
                pageNumber: offset,
                pageSize: limit,
                filter.OrderByValue,
                include: src => src.Include(t => t.Attachment),
                cancellationToken: cancellationToken);

            var data = Mapper.Map<IEnumerable<Entities.Tracker.InspectionAttachment>, IEnumerable<InspectionAttachDto>>(query.Result);
            return new PagingResult(filter.PageNumber, filter.PageSize, query.Item1, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }

        public async Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default)
        {
            var limit = filter.PageSize;
            var offset = --filter.PageNumber * filter.PageSize;

            var predicate = DropDownPredicateBuilderFunction(filter.Filter);
            var query = await UnitOfWork.Repository.FindPagedAsync(predicate: predicate, pageNumber: offset, pageSize: limit, cancellationToken: cancellationToken);

            var data = Mapper.Map<IEnumerable<Entities.Tracker.InspectionAttachment>, IEnumerable<InspectionAttachDto>>(query.Item2.Where(x => x.IsDeleted != true));
            return new PagingResult(filter.PageNumber, filter.PageSize, query.Item1, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }

        public async Task<IFinalResult> GetAllFilterAsync(InspectionAttachFilter filter, CancellationToken cancellationToken = default)
        {
            var predicate = PredicateBuilderFunction(filter);
            var entities = await UnitOfWork.Repository.FindAsync(predicate: predicate, cancellationToken: cancellationToken,
                include: src => src.Include(t => t.Attachment));

            var mapped = Mapper.Map<IEnumerable<Entities.Tracker.InspectionAttachment>, IEnumerable<InspectionAttachDto>>(entities);
            return ResponseResult.PostResult(mapped, status: HttpStatusCode.OK, message: MessagesConstants.Success);
        }

        public override async Task<IFinalResult> AddAsync(AddInspectionAttachDto model, CancellationToken cancellationToken = default)
        {
            var entity = Mapper.Map<Entities.Tracker.InspectionAttachment>(model);

            var result = await UnitOfWork.Repository.AddAsync(entity);
            var affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);

            if (affectedRows <= 0) return ResponseResult.PostResult(false, HttpStatusCode.BadRequest, null, MessagesConstants.AddError);

            return ResponseResult.PostResult(result: result.Id, HttpStatusCode.Created, null, MessagesConstants.AddSuccess);
        }

        public override async Task<IFinalResult> UpdateAsync(AddInspectionAttachDto model, CancellationToken cancellationToken = default)
        {
            Entities.Tracker.InspectionAttachment entityToUpdate = await UnitOfWork.Repository.GetAsync(model.Id);
            var entity = Mapper.Map(model, entityToUpdate);

            UnitOfWork.Repository.Update(entityToUpdate, entity);
            var affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);

            if (affectedRows <= 0) return ResponseResult.PostResult(false, HttpStatusCode.BadRequest, null, MessagesConstants.AddError);

            return ResponseResult.PostResult(true, HttpStatusCode.OK, null, MessagesConstants.UpdateSuccess);
        }

        static Expression<Func<Entities.Tracker.InspectionAttachment, bool>> PredicateBuilderFunction(InspectionAttachFilter filter)
        {
            var predicate = PredicateBuilder.New<Entities.Tracker.InspectionAttachment>(x => x.IsDeleted != true);

            if (!string.IsNullOrWhiteSpace(filter.InspectionId))
            {
                predicate = predicate.And(x => x.InspectionId == filter.InspectionId);
            }

            return predicate;
        }

        static Expression<Func<Entities.Tracker.InspectionAttachment, bool>> DropDownPredicateBuilderFunction(SearchCriteriaFilter filter)
        {
            var predicate = PredicateBuilder.New<Entities.Tracker.InspectionAttachment>(x => !x.IsDeleted);
            return predicate;
        }

        public async Task<IFinalResult> DeleteRangeWithAttachIdRangeAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
        {
            var entitiesToDelete = await UnitOfWork.Repository.FindAsync(d => ids.Contains(d.AttachmentId), cancellationToken: cancellationToken);

            UnitOfWork.Repository.RemoveRange(entitiesToDelete);
            await UnitOfWork.SaveChangesAsync(cancellationToken);

            return entitiesToDelete == null
                ? ResponseResult.PostResult(false, status: HttpStatusCode.BadRequest, message: MessagesConstants.DeleteError)
                : ResponseResult.PostResult(true, status: HttpStatusCode.OK, message: MessagesConstants.DeleteSuccess);
        }
    }
}
