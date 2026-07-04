using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Constants.Auth;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Housing.Leader;
using SonoBooking.Common.DTO.Housing.Leader.Parameters;
using SonoBooking.Domain;
using SonoBooking.Domain.Entities.Housing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Housing.Leaders
{
    public class LeaderService(IServiceBaseParameter<Leader> businessBaseParameter) : BaseService<Leader, AddLeaderDto, EditLeaderDto, LeaderDto, string, string>(businessBaseParameter), ILeaderService
    {
        public override async Task<IFinalResult> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            IFinalResult result = await base.GetByIdAsync(id, cancellationToken);
            if (result?.Data is LeaderDto dto)
                PopulateFileContent(dto, await UnitOfWork.Repository.GetAsync(cancellationToken, id));

            return result;
        }

        public override async Task<IFinalResult> AddAsync(AddLeaderDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                Leader entity = Mapper.Map<AddLeaderDto, Leader>(model);
                await ApplyFileContentAsync(entity, model.File, cancellationToken);
                SetEntityCreatedBaseProperties(entity);

                await UnitOfWork.Repository.AddAsync(entity, cancellationToken);
                int affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);

                if (affectedRows <= 0)
                    return ResponseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: null,
                        message: MessagesConstants.AddError);

                return ResponseResult.PostResult(result: new { Id = entity.Id }, status: HttpStatusCode.Created, exception: null,
                    message: MessagesConstants.AddSuccess);
            }
            catch (Exception ex)
            {
                return ResponseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: ex,
                    message: MessagesConstants.AddError);
            }
        }

        public override async Task<IFinalResult> UpdateAsync(AddLeaderDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                Leader entityToUpdate = await UnitOfWork.Repository.GetAsync(cancellationToken, model.Id);
                if (entityToUpdate == null)
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.NotFound, exception: null,
                        message: MessagesConstants.NotFound);

                Leader entity = Mapper.Map(model, entityToUpdate);
                await ApplyFileContentAsync(entity, model.File, cancellationToken);

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

        public async Task<PagingResult> GetAllPagedAsync(BaseParam<LeaderFilter> filter, CancellationToken cancellationToken = default)
        {
            int pageNumber = filter.PageNumber;
            int limit = filter.PageSize;
            int offset = (pageNumber - 1) * limit;
            LeaderFilter leaderFilter = filter?.Filter ?? new LeaderFilter();

            (int Count, IEnumerable<Leader> Result) = await UnitOfWork.Repository.FindPagedAsync(
                predicate: x => x.IsDeleted == leaderFilter.IsDeleted,
                pageNumber: offset,
                pageSize: limit,
                filter.OrderByValue,
                cancellationToken: cancellationToken);

            IEnumerable<LeaderDto> data = Mapper.Map<IEnumerable<Leader>, IEnumerable<LeaderDto>>(Result ?? []);
            return new PagingResult(pageNumber, limit, Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }

        private static async Task ApplyFileContentAsync(Leader entity, Microsoft.AspNetCore.Http.IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
                return;

            await using MemoryStream memoryStream = new();
            await file.CopyToAsync(memoryStream, cancellationToken);
            entity.FileContent = memoryStream.ToArray();
        }

        private static void PopulateFileContent(LeaderDto dto, Leader entity)
        {
            if (entity?.FileContent == null || entity.FileContent.Length == 0)
                return;

            dto.HasFileContent = true;
            dto.FileContentBase64 = Convert.ToBase64String(entity.FileContent);
        }
    }
}
