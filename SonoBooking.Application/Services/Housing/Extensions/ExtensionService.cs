using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Core;
using SonoBooking.Domain;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Housing.Extension;
using SonoBooking.Common.DTO.Housing.Extension.Parameters;
using SonoBooking.Domain.Entities.Housing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Housing.Extensions
{
    public class ExtensionService(IServiceBaseParameter<Extension> businessBaseParameter)
        : BaseService<Extension, AddExtensionDto, EditExtensionDto, ExtensionDto, string, string>(businessBaseParameter),
            IExtensionService
    {
        public override async Task<IFinalResult> GetAllAsync(
            bool disableTracking = false,
            Expression<Func<Extension, bool>> predicate = null,
            CancellationToken cancellationToken = default)
        {
            string userId = GetUserIdFromHeader();

            IEnumerable<Extension> query;
            if (!string.IsNullOrWhiteSpace(userId))
            {
                string trimmedUserId = userId.Trim();
                query = await UnitOfWork.Repository.FindAsync(
                    x => x.UserId == trimmedUserId,
                    disableTracking: disableTracking,
                    include: source => source.Include(e => e.Reservation),
                    cancellationToken: cancellationToken);
            }
            else if (predicate != null)
            {
                query = await UnitOfWork.Repository.FindAsync(
                    predicate,
                    disableTracking: disableTracking,
                    include: source => source.Include(e => e.Reservation),
                    cancellationToken: cancellationToken);
            }
            else
            {
                query = await UnitOfWork.Repository.GetAllAsync(
                    disableTracking: disableTracking,
                    include: source => source.Include(e => e.Reservation),
                    cancellationToken: cancellationToken);
            }

            IEnumerable<ExtensionDto> data = Mapper.Map<IEnumerable<Extension>, IEnumerable<ExtensionDto>>(query);
            IEnumerable<ExtensionDto> sorted = data.OrderByDescending(r => r.CreatedAt);
            return ResponseResult.PostResult(result: sorted, status: HttpStatusCode.OK, exception: null,
                message: MessagesConstants.Success);
        }

        public async Task<PagingResult> GetAllPagedAsync(BaseParam<ExtensionFilter> filter, CancellationToken cancellationToken = default)
        {
            int pageNumber = filter.PageNumber;
            int limit = filter.PageSize;
            int offset = (pageNumber - 1) * limit;
            ExtensionFilter extensionFilter = filter?.Filter ?? new ExtensionFilter();

            (int Count, IEnumerable<Extension> Result) = await UnitOfWork.Repository.FindPagedAsync(
                predicate: x => x.IsDeleted == extensionFilter.IsDeleted,
                pageNumber: offset,
                pageSize: limit,
                filter.OrderByValue,
                cancellationToken: cancellationToken);

            IEnumerable<ExtensionDto> data = Mapper.Map<IEnumerable<Extension>, IEnumerable<ExtensionDto>>(Result ?? []);
            return new PagingResult(pageNumber, limit, Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }

        public override async Task<IFinalResult> AddAsync(AddExtensionDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                Extension entity = Mapper.Map<AddExtensionDto, Extension>(model);
                SetEntityCreatedBaseProperties(entity);
                await UnitOfWork.Repository.AddAsync(entity, cancellationToken);
                var affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);
                if (affectedRows > 0)
                {
                    Result = ResponseResult.PostResult(result: null, status: HttpStatusCode.Created, exception: null,
                        message: MessagesConstants.AddSuccess);
                    Result.Data = new { Id = entity.Id };
                }

                return Result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override async Task<IFinalResult> UpdateAsync(AddExtensionDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                Extension entityToUpdate = await UnitOfWork.Repository.GetAsync(cancellationToken, model.Id);
                Extension entity = Mapper.Map(model, entityToUpdate);
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
                return ResponseResult.PostResult(result: false, status: HttpStatusCode.BadRequest, exception: ex,
                    message: MessagesConstants.UpdateError);
            }
        }

        private string GetUserIdFromHeader() =>
            HttpContextAccessor?.HttpContext?.Request.Headers["UserId"].FirstOrDefault()?.Trim();
    }
}
