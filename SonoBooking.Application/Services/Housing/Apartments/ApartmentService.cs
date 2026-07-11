using Microsoft.EntityFrameworkCore;
using SonoBooking.Application.Services.Base;
using SonoBooking.Application.Services.LookUp.Attachments;
using SonoBooking.Common.Constants.Auth;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Housing.Apartment;
using SonoBooking.Common.DTO.Housing.Apartment.Parameters;
using SonoBooking.Application.Services.Housing.UnitImages;
using SonoBooking.Application.Services.BackgroundJobs.Housing.Units;
using SonoBooking.Common.DTO.Lookup.ApartmentType;
using SonoBooking.Common.DTO.Lookup.Attachment;
using SonoBooking.Domain;
using SonoBooking.Domain.Entities.Housing;
using SonoBooking.Domain.Entities.Lookups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Housing.Apartments
{
    public class ApartmentService(
           IServiceBaseParameter<Apartment> businessBaseParameter, 
           IAttachmentService attachmentService,
           IUnitAdministrativeStatusJobScheduler administrativeStatusJobScheduler) : 
           BaseService<Apartment, AddApartmentDto, EditApartmentDto, ApartmentDto, string, string>(businessBaseParameter), IApartmentService
    {
        public override async Task<IFinalResult> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            var entity = await UnitOfWork.Repository.FirstOrDefaultAsync(x => x.Id.Equals(id),
                               include: src => src.Include(c => c.UnitImages).ThenInclude(c => c.Attachment)
                               , cancellationToken: cancellationToken);

            ApartmentDto mapped = Mapper.Map<Apartment, ApartmentDto>(entity);

            mapped.Images = UnitImageDtoMapper.MapFromEntities(entity?.UnitImages);


            return ResponseResult.PostResult
                   (result: mapped, status: HttpStatusCode.OK, exception: null,
                    message: MessagesConstants.Success);
        }
        public override async Task<IFinalResult> GetAllAsync(bool disableTracking = false, Expression<Func<Apartment, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            bool isSuperAdmin = IsSuperAdmin();

            IEnumerable<Apartment> entities = await UnitOfWork.Repository.FindAsync(
                predicate: predicate,
                disableTracking: disableTracking,
                include: source => source
                    .Include(a => a.ApartmentType)
                    .Include(a => a.Governorate)
                    .Include(a => a.City)
                    .Include(a => a.UnitImages)
                    .ThenInclude(ui => ui.Attachment),
                cancellationToken: cancellationToken);

            IEnumerable<Apartment> filteredEntities = isSuperAdmin
                ? entities
                : entities.Where(e => !e.IsDeleted);

            var entityList = filteredEntities.ToList();
            List<ApartmentDto> mapped =
                Mapper.Map<List<Apartment>, List<ApartmentDto>>(entityList);

            for (var i = 0; i < entityList.Count; i++)
                mapped[i].Images = UnitImageDtoMapper.MapFromEntities(entityList[i].UnitImages);

            return ResponseResult.PostResult(result: mapped, status: HttpStatusCode.OK, exception: null,
                                             message: HttpStatusCode.OK.ToString());
        }
        public async Task<PagingResult> GetAllPagedAsync(BaseParam<ApartmentFilter> filter, CancellationToken cancellationToken = default)
        {
            int pageNumber = filter.PageNumber;
            int limit = filter.PageSize;
            int offset = (pageNumber - 1) * limit;
            ApartmentFilter apartmentFilter = filter?.Filter ?? new ApartmentFilter();

            (int Count, IEnumerable<Apartment> Result) = await UnitOfWork.Repository.FindPagedAsync(
                predicate: x => x.IsDeleted == apartmentFilter.IsDeleted,
                pageNumber: offset,
                pageSize: limit,
                filter.OrderByValue,
                cancellationToken: cancellationToken);

            IEnumerable<ApartmentDto> data = Mapper.Map<IEnumerable<Apartment>, IEnumerable<ApartmentDto>>(Result ?? []);
            return new PagingResult(pageNumber, limit, Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }
        public override async Task<IFinalResult> AddAsync(AddApartmentDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                //int nextNumber = await UnitOfWork.Repository.Count(
                //    cancellationToken: cancellationToken) + 1;

                //model.ApartmentNumber = $"A{nextNumber:D2}";

                Apartment entity = Mapper.Map<Apartment>(model);

                if (model.Images != null)
                {
                    foreach (var formFile in model.Images)
                    {
                        var AddDto = new AddAttachmentDto
                        {
                            Id = Guid.NewGuid().ToString(),
                            File = formFile.Image,
                            AttachFolder = "Apartments",
                        };

                        IFinalResult Attach = await attachmentService.AddAsync(AddDto, cancellationToken);

                        entity.UnitImages.Add(new UnitImage
                        {
                            AttachmentId = Attach.Data.ToString(),
                            ApartmentId = entity.Id,
                            IsPrimary = formFile.IsPrimary
                        });
                    }
                }

                SetEntityCreatedBaseProperties(entity);

                entity.AdministrativeStatusJobId = administrativeStatusJobScheduler.SyncAdministrativeStatusJob(
                    entity.AdministrativeStatus,
                    null,
                    entity.EndAdministrativeDate,
                    entity.Id,
                    HousingUnitType.Apartment);

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

        public override async Task<IFinalResult> UpdateAsync(AddApartmentDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                Apartment entityToUpdate = await UnitOfWork.Repository.FirstOrDefaultAsync(
                    x => x.Id.Equals(model.Id),
                    include: src => src.Include(c => c.UnitImages),
                    disableTracking: false,
                    cancellationToken: cancellationToken);

                if (entityToUpdate == null)
                    return ResponseResult.PostResult
                           (result: null, status: HttpStatusCode.NotFound, exception: null,
                            message: MessagesConstants.NotFound + "Apartment !");

                Apartment entity = Mapper.Map(model, entityToUpdate);

                if(IsSuperAdmin())
                    entity.IsDeleted = false;


                if (model.Images != null)
                {
                    foreach (var formFile in model.Images)
                    {
                        var AddDto = new AddAttachmentDto
                        {
                            Id = Guid.NewGuid().ToString(),
                            File = formFile.Image,
                            AttachFolder = "Apartments",
                        };
                        
                        var Attach = await attachmentService.AddAsync(AddDto, cancellationToken);
                        
                        entity.UnitImages.Add(new UnitImage
                        {
                            AttachmentId = Attach.Data.ToString(),
                            ApartmentId = entity.Id,
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

                entity.AdministrativeStatusJobId = administrativeStatusJobScheduler.SyncAdministrativeStatusJob(
                    entity.AdministrativeStatus,
                    entityToUpdate.AdministrativeStatusJobId,
                    entity.EndAdministrativeDate,
                    entity.Id,
                    HousingUnitType.Apartment);

                UnitOfWork.Repository.Update(entityToUpdate, entity);

                int affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);

                if (affectedRows < 0)
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
