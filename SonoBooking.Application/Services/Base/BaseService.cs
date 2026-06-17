using AutoMapper;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SonoBooking.Domain.Entities.Base;
using SonoBooking.Common.Constants.Auth;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Identity.User;
using SonoBooking.Common.Infrastructure.UnitOfWork;
using SonoTracker.Domain;
using SonoBooking.Integration.CacheRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using SonoBooking.Domain;

namespace SonoBooking.Application.Services.Base
{
    public class BaseService<T, TAddDto,TEditDto, TGetDto, TKey, TKeyDto>
        : IBaseService<T, TAddDto, TEditDto, TGetDto, TKey, TKeyDto>
        where T : BaseEntity<TKey>
        where TAddDto : IEntityDto<TKeyDto>
        where TEditDto : IEntityDto<TKeyDto>
        where TGetDto : IEntityDto<TKeyDto>
    {
        protected readonly IUnitOfWork<T> UnitOfWork;
        protected readonly IMapper Mapper;
        protected readonly IResponseResult ResponseResult;
        protected IFinalResult Result;
        protected IHttpContextAccessor HttpContextAccessor;
        protected IConfiguration Configuration;
        protected ICacheRepository CacheRepository;
        protected List<string> AppCodes = [];
        private readonly ILogger _logger;
        private readonly JsonSerializerSettings _serializerSettings = new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        protected TokenClaimDto ClaimData { get; set; }
        protected UserDataDto _user;

        /// <summary>
        /// Determines whether the current authenticated user has the `SuperAdmin` role.
        /// </summary>
        /// <returns>True when the current role is SuperAdmin; otherwise false.</returns>
        protected bool IsSuperAdmin()
        {
            return string.Equals(_user.Role, RoleNames.SuperAdmin, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets the current user's GovernorateId from JWT claims.
        /// Returns empty string when claim does not exist.
        /// </summary>
        protected string GetGovernorateIdFromClaims()
        {
            return _user.GovernorateId ?? string.Empty;
        }

        protected internal BaseService(IServiceBaseParameter<T> businessBaseParameter)
        {
            _logger = businessBaseParameter.Logger ?? throw new(nameof(businessBaseParameter.Logger));
            HttpContextAccessor = businessBaseParameter.HttpContextAccessor;
            UnitOfWork = businessBaseParameter.UnitOfWork;
            ResponseResult = businessBaseParameter.ResponseResult;
            Mapper = businessBaseParameter.Mapper;
            CacheRepository = businessBaseParameter.CacheRepository;
            Configuration = businessBaseParameter.Configuration;
            var claims = HttpContextAccessor?.HttpContext?.User;
            var ip = HttpContextAccessor?.HttpContext?.Connection.RemoteIpAddress?.ToString();
            ClaimData = GetTokenClaimDto(claims);
            _user = GetTokenDataDto(claims);
            ClaimData.IpAddress = ip;
        }
    
        public virtual async Task<IFinalResult> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            T query = await UnitOfWork.Repository.GetAsync(cancellationToken, id);
            var data = Mapper.Map<T, TGetDto>(query);
            return ResponseResult.PostResult(result: data, status: HttpStatusCode.OK, exception: null,
                                             message: MessagesConstants.Success);

        }

        public virtual async Task<IFinalResult> GetByIdForEditAsync(object id, CancellationToken cancellationToken = default)
        {
            T query = await UnitOfWork.Repository.GetAsync(cancellationToken, id);
            var data = Mapper.Map<T, TEditDto>(query);
            return ResponseResult.PostResult(result: data, status: HttpStatusCode.OK, exception: null,
                                             message: MessagesConstants.Success);
        }

        public virtual async Task<IFinalResult> GetAllAsync(bool disableTracking = false, Expression<Func<T, bool>> predicate = null, CancellationToken cancellationToken = default)
        {

            IEnumerable<T> query;
            if (predicate != null)
            {
                query = await UnitOfWork.Repository.FindAsync(predicate, cancellationToken: cancellationToken);
            }
            else
            {
                query = await UnitOfWork.Repository.GetAllAsync(disableTracking: disableTracking, cancellationToken: cancellationToken);
            }

            var data = Mapper.Map<IEnumerable<T>, IEnumerable<TGetDto>>(query);
            return ResponseResult.PostResult(result: data, status: HttpStatusCode.OK, exception: null,
                                             message: HttpStatusCode.OK.ToString());

        }

        public virtual async Task<IFinalResult> AddAsync(TAddDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                T entity = Mapper.Map<TAddDto, T>(model);
                //SetEntityCreatedBaseProperties(entity);
                await UnitOfWork.Repository.AddAsync(entity, cancellationToken);
                var affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);
                if (affectedRows > 0)
                {
                    Result = new ResponseResult(result: null, status: HttpStatusCode.Created, exception: null,
                                                message: MessagesConstants.AddSuccess);
                    Result.Data = new
                    {
                        Id = entity.Id, // Adjust this according to your entity's ID property
                      
                    };
                }

                return Result;
            }
            catch (Exception e)
            {
                _logger.LogError($"{MessagesConstants.AddError}-{nameof(AddAsync)}");
                _logger.LogError(JsonConvert.SerializeObject(e, _serializerSettings));
                throw;
            }

        }
        
        public virtual async Task<IFinalResult> AddListAsync(List<TAddDto> model, CancellationToken cancellationToken = default)
        {

            try
            {
                List<T> entities = Mapper.Map<List<TAddDto>, List<T>>(model);
                UnitOfWork.Repository.AddRange(entities);
                var affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);
                if (affectedRows > 0)
                {
                    Result = new ResponseResult(result: null, status: HttpStatusCode.Created, exception: null,
                                                message: MessagesConstants.AddError);
                }
                Result.Data = model;
                return Result;
            }
            catch (Exception e)
            {
                _logger.LogError($"{MessagesConstants.AddError}-{nameof(AddListAsync)}");
                _logger.LogError(JsonConvert.SerializeObject(e, _serializerSettings));
                throw;
            }

        }
        
        public virtual async Task<IFinalResult> UpdateAsync(TAddDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                T entityToUpdate = await UnitOfWork.Repository.GetAsync(cancellationToken, model.Id);

                var newEntity = Mapper.Map(model, entityToUpdate);

                UnitOfWork.Repository.Update(entityToUpdate, newEntity);

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
                //_logger.LogError($"{MessagesConstants.UpdateError}-{nameof(UpdateAsync)}");
                //_logger.LogError(JsonConvert.SerializeObject(e, _serializerSettings));
                //throw;
            }
        }
       
        public virtual async Task<IFinalResult> DeleteAsync(object id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entityToDelete = await UnitOfWork.Repository.GetAsync(cancellationToken, id);

                UnitOfWork.Repository.Remove(entityToDelete);
               
                int affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);
                
                if (affectedRows <= 0)
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.BadRequest, exception: null,
                                                     message: MessagesConstants.DeleteError);

                return ResponseResult.PostResult(result: true, status: HttpStatusCode.Accepted, exception: null,
                                                 message: MessagesConstants.DeleteSuccess);
            }
            catch (Exception e)
            {
                return ResponseResult.PostResult(result: false, status: HttpStatusCode.BadRequest, exception: e,
                                                 message: MessagesConstants.DeleteError);
                //_logger.LogError($"{MessagesConstants.DeleteError}-{nameof(DeleteAsync)}");
                //_logger.LogError(JsonConvert.SerializeObject(e, _serializerSettings));
                //throw;
            }
        }
      
        public virtual async Task<IFinalResult> DeleteSoftAsync(object id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entityToDelete = await UnitOfWork.Repository.GetAsync(cancellationToken, id);

                UnitOfWork.Repository.RemoveLogical(entityToDelete);
                
                int affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);

                if (affectedRows <= 0)
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.BadRequest, exception: null,
                                                     message: MessagesConstants.DeleteError);

                return ResponseResult.PostResult(result: true, status: HttpStatusCode.Accepted, exception: null,
                                                   message: MessagesConstants.DeleteSuccess);
            }
            catch (Exception e)
            {
                return ResponseResult.PostResult(result: false, status: HttpStatusCode.BadRequest, exception: e,
                                                 message: MessagesConstants.DeleteError);
                //_logger.LogError($"{MessagesConstants.DeleteError}-{nameof(DeleteSoftAsync)}");
                //_logger.LogError(JsonConvert.SerializeObject(e, _serializerSettings));
                //throw;
            }

        }

        public virtual async Task<IFinalResult> GetLastRecordAsync(CancellationToken cancellationToken = default)
        {
            T query = await UnitOfWork.Repository.GetLast(cancellationToken);
            var data = Mapper.Map<T, TGetDto>(query);
            return ResponseResult.PostResult(result: data, status: HttpStatusCode.OK, exception: null,
                                             message: MessagesConstants.Success);
        }

        protected virtual void SetEntityCreatedBaseProperties(BaseAudit<TKey> entity)
        {
            entity.CreatedById = _user.Id != "" ? _user.Id : "System";
            entity.CreatedBy = _user.Name != "" ? _user.Name : "System";
            entity.CreatedAt = DateTime.UtcNow;
            entity.ModifiedById = _user.Id != "" ? _user.Id : "System";
            entity.ModifiedBy = _user.Name != "" ? _user.Name : "System";
            entity.ModifiedAt = DateTime.UtcNow;
            entity.IpAddress = ClaimData.IpAddress;
        }

        protected virtual void SetEntityModifiedBaseProperties(BaseAudit<TKey> entity)
        {
            entity.ModifiedById = _user.Id != "" ? _user.Id : "System";
            entity.ModifiedBy = _user.Name != "" ? _user.Name : "System";
            entity.ModifiedAt = DateTime.UtcNow;
            entity.IpAddress = ClaimData.IpAddress;
        }

        //protected virtual async Task<List<RoleDto>> GetRoles(string nationalId)
        //{
        //    AppCodes.Add(Configuration["AppCodes:ReviewersServices"]);
        //    var result = await CacheRepository.GetEmployeeAsync(nationalId);
        //    var roles = result.Roles.Where(x => AppCodes.Contains(x.AppCode));
        //    return roles.ToList();
        //}

        private TokenClaimDto GetTokenClaimDto(ClaimsPrincipal claims)
        {
            var claimData = new TokenClaimDto()
            {
                UserId = claims?.FindFirst(t => t.Type == ClaimTypes.NameIdentifier)?.Value,
                EmployeeId = claims?.FindFirst(t => t.Type == ClaimTypes.Name)?.Value,
                EmployeeEn = claims?.FindFirst(t => t.Type == "EmployeeEn")?.Value,
                EmployeeAr = claims?.FindFirst(t => t.Type == "EmployeeAr")?.Value,
                UnitId = claims?.FindFirst(t => t.Type == "UnitId")?.Value,
                TeamId = claims?.FindFirst(t => t.Type == "TeamId")?.Value,
                NationalId = claims?.FindFirst(t => t.Type == "NationalId")?.Value
            };
            return claimData;
        }

        private static UserDataDto GetTokenDataDto(ClaimsPrincipal claims)
        {
            var claimData = new UserDataDto(
                claims?.FindFirst(t => t.Type == ClaimTypes.NameIdentifier)?.Value,
                claims?.FindFirst(t => t.Type == ClaimTypes.Name)?.Value,
                claims?.FindFirst(t => t.Type == ClaimTypes.Role)?.Value, [], "", "",
                claims?.FindFirst(x => x.Type == AuthConstants.GovId)?.Value,
                claims?.FindFirst(x => x.Type == AuthConstants.EmployeeId)?.Value);
            return claimData;
        }
    }
}

