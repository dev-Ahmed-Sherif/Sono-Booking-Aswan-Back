using Microsoft.EntityFrameworkCore;
using SonoBooking.Application.Services.Base;
using SonoBooking.Application.Services.LookUp.Attachments;
using SonoBooking.Common.Constants.Auth;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Housing.Apartment;
using SonoBooking.Common.DTO.Housing.Bed;
using SonoBooking.Common.DTO.Housing.Bed.Parameters;
using SonoBooking.Common.DTO.Housing.UnitImage;
using SonoBooking.Common.DTO.Lookup.Attachment;
using SonoBooking.Domain;
using SonoBooking.Domain.Entities.Housing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Housing.Beds
{
    public class BedService(
                 IServiceBaseParameter<Bed> businessBaseParameter,
                 IAttachmentService attachmentService) : BaseService<Bed, AddBedDto, EditBedDto, BedDto, string, string>(businessBaseParameter), IBedService
    {
        public override async Task<IFinalResult> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            var entity = await UnitOfWork.Repository.FirstOrDefaultAsync(x => x.Id.Equals(id),
                               include: src => src.Include(c => c.UnitImages).ThenInclude(c => c.Attachment)
                               , cancellationToken: cancellationToken);

            BedDto mapped = Mapper.Map<Bed, BedDto>(entity);

            mapped.Images = [.. entity.UnitImages.Select(a => new UnitImageDto
                                        {
                                            Id = a.Id,
                                            AttachmentId = a.AttachmentId,
                                            BedId = a.BedId,
                                            Name = a.Attachment.FileName,
                                            Url = a.Attachment.Url,
                                            IsPrimary = a.IsPrimary
                                        })];


            return ResponseResult.PostResult
                   (result: mapped, status: HttpStatusCode.OK, exception: null,
                    message: MessagesConstants.Success);
        }
        public override async Task<IFinalResult> GetAllAsync(bool disableTracking = false, Expression<Func<Bed, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            bool isSuperAdmin = IsSuperAdmin();

            IEnumerable<Bed> entities = await UnitOfWork.Repository.FindAsync(
                predicate: predicate,
                disableTracking: disableTracking,
                include: source => source
                    .Include(b => b.Room),
                cancellationToken: cancellationToken);

            IEnumerable<Bed> filteredEntities = isSuperAdmin
                ? entities
                : entities.Where(e => !e.IsDeleted);

            IEnumerable<BedDto> mapped =
                Mapper.Map<IEnumerable<Bed>, IEnumerable<BedDto>>(filteredEntities);

            return ResponseResult.PostResult(result: mapped, status: HttpStatusCode.OK, exception: null,
                                             message: HttpStatusCode.OK.ToString());
        }
        public async Task<PagingResult> GetAllPagedAsync(BaseParam<BedFilter> filter, CancellationToken cancellationToken = default)
        {
            int pageNumber = filter.PageNumber;
            int limit = filter.PageSize;
            int offset = (pageNumber - 1) * limit;
            BedFilter bedFilter = filter?.Filter ?? new BedFilter();

            (int Count, IEnumerable<Bed> Result) = await UnitOfWork.Repository.FindPagedAsync(
                predicate: x => x.IsDeleted == bedFilter.IsDeleted,
                pageNumber: offset,
                pageSize: limit,
                filter.OrderByValue,
                cancellationToken: cancellationToken);

            IEnumerable<BedDto> data = Mapper.Map<IEnumerable<Bed>, IEnumerable<BedDto>>(Result ?? []);
            return new PagingResult(pageNumber, limit, Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }
        public override async Task<IFinalResult> AddAsync(AddBedDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                int nextNumber = await UnitOfWork.Repository.Count(
                    b => b.RoomId == model.RoomId, cancellationToken) + 1;

                model.BedNumber = $"B{nextNumber:D2}";

                Bed entity = Mapper.Map<Bed>(model);

                if (model.Images != null)
                {
                    foreach (var formFile in model.Images)
                    {
                        var AddDto = new AddAttachmentDto
                        {
                            Id = Guid.NewGuid().ToString(),
                            File = formFile.Image,
                            AttachFolder = "Beds",
                        };

                        IFinalResult Attach = await attachmentService.AddAsync(AddDto, cancellationToken);

                        entity.UnitImages.Add(new UnitImage
                        {
                            AttachmentId = Attach.Data.ToString(),
                            BedId = entity.Id,
                            IsPrimary = formFile.IsPrimary
                        });
                    }
                }

                SetEntityCreatedBaseProperties(entity);

                await UnitOfWork.Repository.AddAsync(entity, cancellationToken);

                var affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);

                if (affectedRows <= 0)
                    return ResponseResult.PostResult
                           (result: null, status: HttpStatusCode.BadRequest, exception: null,
                            message: MessagesConstants.AddError);

                return ResponseResult.PostResult
                       (result: entity.Id, status: HttpStatusCode.Created, exception: null,
                        message: MessagesConstants.AddSuccess);
            }
            catch (Exception ex)
            {
                return ResponseResult.PostResult
                       (result: null, status: HttpStatusCode.BadRequest, exception: ex,
                        message: MessagesConstants.AddError);
            }
        }

        public override async Task<IFinalResult> UpdateAsync(AddBedDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                Bed entityToUpdate = await UnitOfWork.Repository.FirstOrDefaultAsync(
                    x => x.Id.Equals(model.Id),
                    include: src => src.Include(c => c.UnitImages),
                    disableTracking: false,
                    cancellationToken: cancellationToken);

                if (entityToUpdate == null)
                    return ResponseResult.PostResult
                           (result: null, status: HttpStatusCode.NotFound, exception: null,
                            message: MessagesConstants.NotFound + "Bed !");

                Bed entity = Mapper.Map(model, entityToUpdate);

                if (model.Images != null)
                {
                    foreach (var formFile in model.Images)
                    {
                        var AddDto = new AddAttachmentDto
                        {
                            Id = Guid.NewGuid().ToString(),
                            File = formFile.Image,
                            AttachFolder = "Beds",
                        };

                        var Attach = await attachmentService.AddAsync(AddDto, cancellationToken);

                        entity.UnitImages.Add(new UnitImage
                        {
                            AttachmentId = Attach.Data.ToString(),
                            BedId = entity.Id,
                            IsPrimary = formFile.IsPrimary
                        });
                    }
                }

                if (model.OldImages != null)
                {
                    foreach (var oldImage in model.OldImages)
                    {
                        UnitImage existingImage = entity.UnitImages
                            .FirstOrDefault(ui => ui.Id == oldImage.Id);

                        if (existingImage != null)
                            existingImage.IsPrimary = oldImage.IsPrimary;
                    }
                }

                SetEntityModifiedBaseProperties(entity);

                UnitOfWork.Repository.Update(entityToUpdate, entity);

                int affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);

                if (affectedRows <= 0)
                    return ResponseResult.PostResult
                           (result: false, status: HttpStatusCode.BadRequest, exception: null,
                            message: MessagesConstants.UpdateError);

                return ResponseResult.PostResult
                       (result: true, status: HttpStatusCode.Accepted, exception: null,
                        message: MessagesConstants.UpdateSuccess);
            }
            catch (Exception ex)
            {
                return ResponseResult.PostResult
                       (result: false, status: HttpStatusCode.BadRequest, exception: ex,
                        message: ex.Message);
            }
        }
        public override async Task<IFinalResult> DeleteAsync(object id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entityToDelete = await UnitOfWork.Repository.GetAsync(id, cancellationToken);
                // Remove Uploaded Files
                //need array of attach ids to delete
                List<string> attachIds = [.. entityToDelete.UnitImages.Select(a => a.AttachmentId)];
                IFinalResult finalResult = await attachmentService.DeleteRangeAsync(attachIds, cancellationToken);
                entityToDelete.UnitImages.Clear();

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
                return ResponseResult.PostResult(result: false, status: HttpStatusCode.BadRequest, exception: e,
                                                 message: MessagesConstants.DeleteError);
                //_logger.LogError($"{MessagesConstants.DeleteError}-{nameof(DeleteAsync)}");
                //_logger.LogError(JsonConvert.SerializeObject(e, _serializerSettings));
                throw;
            }

        }
    }
}
