using LinqKit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Lookup.Attachment;
using SonoBooking.Common.DTO.Lookup.Attachment.Parameters;
using SonoBooking.Common.Helpers.MediaUploader;
using SonoBooking.Domain;
using SonoBooking.Domain.Entities.Lookups;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.LookUp.Attachments
{
    public class AttachmentService
        : BaseService<Attachment, AddAttachmentDto, EditAttachmentDto, AttachmentDto, string, string>, IAttachmentService
    {
        private readonly IWebHostEnvironment _webHostingEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UploaderConfiguration _uploaderConfiguration;

        public AttachmentService(IServiceBaseParameter<Attachment> businessBaseParameter,
        IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor) : base(businessBaseParameter)
        {
            _webHostingEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
            _uploaderConfiguration = new(_webHostingEnvironment, _httpContextAccessor);
        }

        public override async Task<IFinalResult> GetAllAsync(bool disableTracking = false, Expression<Func<Attachment, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            var isSuperAdmin = IsSuperAdmin();

            IEnumerable<Attachment> entities = await UnitOfWork.Repository.FindAsync(
                predicate: predicate,
                disableTracking: disableTracking,
                cancellationToken: cancellationToken);

            IEnumerable<Attachment> filteredEntities = isSuperAdmin
                ? (entities ?? [])
                : (entities?.Where(x => !x.IsDeleted) ?? []);

            var mapped = Mapper.Map<IEnumerable<Attachment>, IEnumerable<AttachmentDto>>(filteredEntities);

            return ResponseResult.PostResult(result: mapped, status: HttpStatusCode.OK, exception: null,
                message: HttpStatusCode.OK.ToString());
        }

        public async Task<PagingResult> GetAllPagedAsync(BaseParam<AttachmentFilter> filter, CancellationToken cancellationToken = default)
        {
            var limit = filter.PageSize;
            var offset = --filter.PageNumber * filter.PageSize;
            var attachmentFilter = filter?.Filter ?? new AttachmentFilter();

            (int Count, IEnumerable<Attachment> Result) =
                await UnitOfWork.Repository.FindPagedAsync(
                    predicate: PredicateBuilderFunction(attachmentFilter),
                    pageNumber: offset,
                    pageSize: limit,
                    filter.OrderByValue,
                    cancellationToken: cancellationToken);

            var data = Mapper.Map<IEnumerable<Attachment>, IEnumerable<AttachmentDto>>(Result ?? []);
            return new PagingResult(filter.PageNumber, filter.PageSize, Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }

        public async Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default)
        {
            var limit = filter.PageSize;
            var offset = --filter.PageNumber * filter.PageSize;
            var predicate = DropDownPredicateBuilderFunction(filter.Filter);

            (int Count, IEnumerable<Attachment> Result) query =
                await UnitOfWork.Repository.FindPagedAsync(predicate: predicate, pageNumber: offset, pageSize: limit, cancellationToken: cancellationToken);

            var data = Mapper.Map<IEnumerable<Attachment>, IEnumerable<AttachmentDto>>(query.Result ?? []);
            return new PagingResult(filter.PageNumber, filter.PageSize, query.Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }

        public override async Task<IFinalResult> AddAsync(AddAttachmentDto model, CancellationToken cancellationToken = default)
        {
            var entity = Mapper.Map<Attachment>(model);

            if (model.File != null)
            {
                string res = await _uploaderConfiguration.UploadFile(model.File, $"Attach/{model.AttachFolder}", cancellationToken);

                if (res != null)
                {
                    if (UploadResponse(res) != null)
                        return UploadResponse(res);
                }
                entity.FileName = model.File.FileName;
                entity.Extension = Path.GetExtension(model.File.FileName);
                entity.Url = res;
            }

            await UnitOfWork.Repository.AddAsync(entity, cancellationToken);

            var affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);

            if (affectedRows <= 0) return ResponseResult.PostResult(false, HttpStatusCode.BadRequest, null, MessagesConstants.AddError);

            return ResponseResult.PostResult(result: entity.Id, HttpStatusCode.Created, null, MessagesConstants.AddSuccess);
        }

        public async Task<IFinalResult> DeleteRangeAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
        {
            var entitiesToDelete = await UnitOfWork.Repository.FindAsync(d => ids.Contains(d.Id), cancellationToken: cancellationToken);

            for (int i = 0; i < entitiesToDelete.ToList().Count; i++)
            {
                _uploaderConfiguration.DeleteFile(entitiesToDelete.ToList()[i].Url);
            }

            UnitOfWork.Repository.RemoveRange(entitiesToDelete, cancellationToken);

            var rows = await UnitOfWork.SaveChangesAsync(cancellationToken);

            return entitiesToDelete == null ?
                ResponseResult.PostResult(false, status: HttpStatusCode.BadRequest, message: MessagesConstants.DeleteError) :
                ResponseResult.PostResult(true, status: HttpStatusCode.OK, message: MessagesConstants.DeleteSuccess);
        }

        static Expression<Func<Attachment, bool>> PredicateBuilderFunction(AttachmentFilter filter)
        {
            var predicate = PredicateBuilder.New<Attachment>(x => x.IsDeleted == filter.IsDeleted);
            //if (!string.IsNullOrWhiteSpace(filter.FileName))
            //    predicate = predicate.And(x => x.FileName.Contains(filter.FileName));

            return predicate;
        }

        static Expression<Func<Attachment, bool>> DropDownPredicateBuilderFunction(SearchCriteriaFilter filter)
        {
            var predicate = PredicateBuilder.New<Attachment>(true);
            if (!string.IsNullOrWhiteSpace(filter?.SearchCriteria))
                predicate = predicate.And(x => x.FileName.Contains(filter.SearchCriteria));

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
