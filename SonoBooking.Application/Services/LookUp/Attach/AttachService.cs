using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SonoBooking.Domain.Entities.Lookups;
using SonoTracker.Application.Services.Base;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Lookup.Attach;
using SonoTracker.Common.Helpers.MediaUploader;
using SonoTracker.Domain;

namespace SonoTracker.Application.Services.LookUp.Attach
{
    public class AttachService : BaseService<Attachment, AddAttachDto, EditAttachDto, AttachDto, string, string>, IAttachService
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IHttpContextAccessor _request;
        private readonly UploaderConfiguration _uploaderConfiguration;

        public AttachService(
            IServiceBaseParameter<Attachment> businessBaseParameter,
            IWebHostEnvironment hostingEnvironment,
            IHttpContextAccessor request)
            : base(businessBaseParameter)
        {
            _hostingEnvironment = hostingEnvironment;
            _request = request;
            _uploaderConfiguration = new UploaderConfiguration(_hostingEnvironment, _request);
        }

        public override async Task<IFinalResult> GetByIdForEditAsync(object id, CancellationToken cancellationToken = default)
        {
            var entity = await UnitOfWork.Repository.FirstOrDefaultAsync(x => x.Id == id.ToString(), cancellationToken: cancellationToken);
            var mapped = Mapper.Map<Attachment, EditAttachDto>(entity);
            return ResponseResult.PostResult(mapped, HttpStatusCode.OK);
        }

        public override async Task<IFinalResult> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            var entity = await UnitOfWork.Repository.FirstOrDefaultAsync(x => x.Id == id.ToString(), cancellationToken: cancellationToken);
            var mapped = Mapper.Map<Attachment, AttachDto>(entity);
            return ResponseResult.PostResult(mapped, HttpStatusCode.OK);
        }

        public override async Task<IFinalResult> GetAllAsync(bool disableTracking = false, Expression<Func<Attachment, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            var entity = await UnitOfWork.Repository.GetAllAsync(disableTracking: disableTracking, cancellationToken: cancellationToken);
            var filteredEntities = entity.Where(e => !e.IsDeleted);
            var mapped = Mapper.Map<IEnumerable<Attachment>, IEnumerable<AttachDto>>(filteredEntities);
            return ResponseResult.PostResult(mapped, status: HttpStatusCode.OK, message: HttpStatusCode.OK.ToString());
        }

        public override async Task<IFinalResult> AddAsync(AddAttachDto model, CancellationToken cancellationToken = default)
        {
            var entity = Mapper.Map<Attachment>(model);

            if (model.Path != null)
            {
                string res = await _uploaderConfiguration.UploadFile(model.Path, $"Attach/{model.AttachType}", cancellationToken);

                if (res != null)
                {
                    if (UploadResponse(res) != null)
                        return UploadResponse(res);
                }

                entity.FileName = model.Path.FileName;
                entity.Extension = Path.GetExtension(model.Path.FileName);
                entity.Url = res;
            }

            await UnitOfWork.Repository.AddAsync(entity, cancellationToken);

            var affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);

            if (affectedRows <= 0)
                return ResponseResult.PostResult(false, HttpStatusCode.BadRequest, null, MessagesConstants.AddError);

            return ResponseResult.PostResult(result: entity.Id, HttpStatusCode.Created, null, MessagesConstants.AddSuccess);
        }

        public async Task<IFinalResult> DeleteRangeAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
        {
            var entitiesToDelete = await UnitOfWork.Repository.FindAsync(d => ids.Contains(d.Id), cancellationToken: cancellationToken);

            foreach (var item in entitiesToDelete)
                _uploaderConfiguration.DeleteFile(item.Url);

            UnitOfWork.Repository.RemoveRange(entitiesToDelete);
            var rows = await UnitOfWork.SaveChangesAsync(cancellationToken);

            return rows > 0
                ? ResponseResult.PostResult(true, status: HttpStatusCode.OK, message: MessagesConstants.DeleteSuccess)
                : ResponseResult.PostResult(false, status: HttpStatusCode.BadRequest, message: MessagesConstants.DeleteError);
        }

        private IFinalResult UploadResponse(string res)
        {
            if (res == "Size")
                return ResponseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: null,
                                                 message: "File Size Larger than 5 Mega Bytes");
            if (res == "Type")
                return ResponseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: null, 
                                                 message: "File type not allowed.");
            return null;
        }
    }
}
