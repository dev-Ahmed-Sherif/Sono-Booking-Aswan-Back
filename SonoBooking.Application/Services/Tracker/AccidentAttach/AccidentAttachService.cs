using LinqKit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Reporting.NETCore;
using SonoTracker.Application.Services.Base;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Tracker.AccidentAttach;
using SonoTracker.Common.DTO.Tracker.AccidentAttach.Parameters;
using SonoTracker.Common.Helpers.MediaUploader;
using SonoTracker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SonoTracker.Application.Services.Tracker.AccidentAttach
{
    public class AccidentAttachService : BaseService<Entities.Tracker.AccidentAttachment, AddAccidentAttachDto, EditAccidentAttachDto, AccidentAttachDto, string, string>, IAccidentAttachService
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IHttpContextAccessor _request;
        private readonly UploaderConfiguration _uploaderConfiguration;
        //private UserData _user;
        //private readonly ICorrespondenceService _CorrespondenceService;
        public AccidentAttachService(
            IServiceBaseParameter<Entities.Tracker.AccidentAttachment> businessBaseParameter,
            IWebHostEnvironment hostingEnvironment, IHttpContextAccessor request
            //UserData user, ICorrespondenceService CorrespondenceService
            ) : base(businessBaseParameter)
        {
            _hostingEnvironment = hostingEnvironment;
            _request = request;
            _uploaderConfiguration = new UploaderConfiguration(_hostingEnvironment, _request);
            //_user = user;
            //_CorrespondenceService = CorrespondenceService;
        }
        public override async Task<IFinalResult> GetByIdForEditAsync(object id, CancellationToken cancellationToken = default)
        {
            var entity = await UnitOfWork.Repository.FirstOrDefaultAsync(x => x.Id == id.ToString(), 
                           include: src =>
                            src.Include(t => t.Attachment),cancellationToken: cancellationToken
                               );


            var mapped = Mapper.Map<Entities.Tracker.AccidentAttachment, EditAccidentAttachDto>(entity);
            return ResponseResult.PostResult(mapped, HttpStatusCode.OK);
        }

        public override async Task<IFinalResult> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            var entity = await UnitOfWork.Repository.FirstOrDefaultAsync(x => x.Id == id.ToString(),
                include: src =>
                            src.Include(t => t.Attachment));


            var mapped = Mapper.Map<Entities.Tracker.AccidentAttachment, AccidentAttachDto>(entity);

            return ResponseResult.PostResult(mapped, HttpStatusCode.OK);
        }
        public override async Task<IFinalResult> GetAllAsync(bool disableTracking = false, Expression<Func<Entities.Tracker.AccidentAttachment, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            var entity = await UnitOfWork.Repository.GetAllAsync(
                include: src =>
                            src.Include(t => t.Attachment));

            var filteredEntities = entity.Where(e => !e.IsDeleted);

            IEnumerable<AccidentAttachDto> mapped = Mapper.Map<IEnumerable<Entities.Tracker.AccidentAttachment>, IEnumerable<AccidentAttachDto>>(filteredEntities);
            
            return ResponseResult.PostResult(mapped, status: HttpStatusCode.OK,
                message: HttpStatusCode.OK.ToString());
        }
        public async Task<PagingResult> GetAllPagedAsync(BaseParam<AccidentAttachFilter> filter, CancellationToken cancellationToken = default)
        {
            var limit = filter.PageSize;

            var offset = --filter.PageNumber * filter.PageSize;

            var query = await UnitOfWork.Repository.FindPagedAsync(predicate: PredicateBuilderFunction(filter.Filter),
                pageNumber: offset,
                pageSize: limit,
                filter.OrderByValue,
                include: src =>
                            src.Include(t => t.Attachment), cancellationToken: cancellationToken);


            var data = Mapper.Map<IEnumerable<Entities.Tracker.AccidentAttachment>, IEnumerable<AccidentAttachDto>>(query.Result);
            return new PagingResult(filter.PageNumber, filter.PageSize, query.Item1, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }

        public async Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default)
        {

            var limit = filter.PageSize;

            var offset = --filter.PageNumber * filter.PageSize;

            var predicate = DropDownPredicateBuilderFunction(filter.Filter);

            var query = await UnitOfWork.Repository.FindPagedAsync(predicate: predicate, pageNumber: offset, pageSize: limit, cancellationToken: cancellationToken);

            var data = Mapper.Map<IEnumerable<Entities.Tracker.AccidentAttachment>, IEnumerable<AccidentAttachDto>>(query.Item2.Where(x => x.IsDeleted != true));

            return new PagingResult(filter.PageNumber, filter.PageSize, query.Item1, data, status: HttpStatusCode.OK, MessagesConstants.Success);

        }
        public async Task<IFinalResult> GetAllFilterAsync(AccidentAttachFilter filter, CancellationToken cancellationToken = default)
        {
            var predicate = PredicateBuilderFunction(filter);
            var entities = await UnitOfWork.Repository.FindAsync(predicate: predicate, cancellationToken: cancellationToken,
                include: src =>
                            src.Include(t => t.Attachment));

            var mapped = Mapper.Map<IEnumerable<Entities.Tracker.AccidentAttachment>, IEnumerable<AccidentAttachDto>>(entities);
            return ResponseResult.PostResult(mapped, status: HttpStatusCode.OK, message: MessagesConstants.Success);
        }

        public override async Task<IFinalResult> AddAsync(AddAccidentAttachDto model, CancellationToken cancellationToken = default)
        {

            var entity = Mapper.Map<Entities.Tracker.AccidentAttachment>(model);

            //if (model.UrgencyAttachment != null)
            //{
            //    entity.UrgencyAttachment = await _uploaderConfiguration.UploadFile(model.UrgencyAttachment, "Urgency");
            //}

            var result = await UnitOfWork.Repository.AddAsync(entity);
            var affectedRows = await UnitOfWork.SaveChangesAsync();

            if (affectedRows <= 0) return ResponseResult.PostResult(false, HttpStatusCode.BadRequest, null, MessagesConstants.AddError);

            return ResponseResult.PostResult(result: entity.Id, HttpStatusCode.Created, null, MessagesConstants.AddSuccess);


            //Guid orgGuid = Guid.NewGuid();

            //if (_user.OrganizationId != "")
            //{
            //    orgGuid = Guid.Parse(_user.OrganizationId);
            //}

            //entity.OrganizationId = _user.OrganizationId != "" ? orgGuid : null;

            //var CorrespondenceData = await _CorrespondenceService.GetByIdAsync(model.CorrespondenceId);

            //if (CorrespondenceData.Data == null)
            //{
            //    return ResponseResult.PostResult(false, HttpStatusCode.BadRequest, null
            //        , message: "Correspondence Not Found");
            //}

            //var Correspondence = CorrespondenceData.Data as EditCorrespondenceDto;

            //entity.SentCategory = Correspondence.CorresCategoryId == CorresCategory.Meeting
            //                    ? SentCategory.MeetingRecommendations
            //                    : Correspondence.CorresCategoryId == CorresCategory.Letter
            //                        ? SentCategory.Letter
            //                        : SentCategory.Other;

        }

        public override async Task<IFinalResult> UpdateAsync(AddAccidentAttachDto model, CancellationToken cancellationToken = default)
        {


            Entities.Tracker.AccidentAttachment entityToUpdate = await UnitOfWork.Repository.GetAsync(model.Id);

            var entity = Mapper.Map(model, entityToUpdate);

            //Guid orgGuid = Guid.NewGuid();

            //if (_user.OrganizationId != "")
            //{
            //    orgGuid = Guid.Parse(_user.OrganizationId);
            //}

            //entity.OrganizationId = _user.OrganizationId != "" ? orgGuid : null;

            //var CorrespondenceData = await _CorrespondenceService.GetByIdAsync(model.CorrespondenceId);

            //if (CorrespondenceData.Data == null)
            //{
            //    return ResponseResult.PostResult(false, HttpStatusCode.BadRequest, null
            //        ,message: "Correspondence Not Found");
            //}

            //var Correspondence = CorrespondenceData.Data as EditCorrespondenceDto;

            //entity.SentCategory = Correspondence.CorresCategoryId == CorresCategory.Meeting
            //                    ? SentCategory.MeetingRecommendations
            //                    : Correspondence.CorresCategoryId == CorresCategory.Letter
            //                        ? SentCategory.Letter
            //                        : SentCategory.Other;

            UnitOfWork.Repository.Update(entityToUpdate, entity);

            var affectedRows = await UnitOfWork.SaveChangesAsync();

            if (affectedRows <= 0) return ResponseResult.PostResult(false, HttpStatusCode.BadRequest, null, MessagesConstants.AddError);

            return ResponseResult.PostResult(true, HttpStatusCode.OK, null, MessagesConstants.UpdateSuccess);
        }
        
        static Expression<Func<Entities.Tracker.AccidentAttachment, bool>> PredicateBuilderFunction(AccidentAttachFilter filter)
        {
            var predicate = PredicateBuilder.New<Entities.Tracker.AccidentAttachment>(x => x.IsDeleted != true);

            //if (filter.SentId.HasValue)
            //{
            //    predicate = predicate.And(x => x.SentId == filter.SentId);
            //}
            //if (filter.CorrespondenceId.HasValue)
            //{
            //    predicate = predicate.And(x => x.CorrespondenceId == filter.CorrespondenceId.Value);
            //}
            //if (filter.SentSequenceNumber.HasValue)
            //{
            //    predicate = predicate.And(x => x.SentSequenceNumber == filter.SentSequenceNumber.Value);
            //}
            //if (!string.IsNullOrWhiteSpace(filter?.Title))
            //{
            //    predicate = predicate.And(x => x.Title == filter.Title);
            //}
            if (!string.IsNullOrWhiteSpace(filter.AccidentId))
            {
                predicate = predicate.And(x => x.AccidentId == filter.AccidentId);
            }
           

            return predicate;
        }


        static Expression<Func<Entities.Tracker.AccidentAttachment, bool>> DropDownPredicateBuilderFunction(SearchCriteriaFilter filter)
        {
            var predicate = PredicateBuilder.New<Entities.Tracker.AccidentAttachment>(x => !x.IsDeleted);
            //if (!string.IsNullOrWhiteSpace(filter?.SearchCriteria))
            //{
            //    predicate = predicate.And(b => b.Title.Contains(filter.SearchCriteria));
            //}
            return predicate;
        }
        //TO-DO remove the loop here for finding the entities to be deleted
        // get all entities at once using search first (FindAsync)
        // then use remove range

        public async Task<IFinalResult> DeleteRangeWithAttachIdRangeAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
        {
            var entitiesToDelete = await UnitOfWork.Repository.FindAsync(d => ids.Contains(d.AttachmentId), cancellationToken: cancellationToken);

            UnitOfWork.Repository.RemoveRange(entitiesToDelete);

            var rows = await UnitOfWork.SaveChangesAsync();

            return entitiesToDelete == null ?
                ResponseResult.PostResult(false, status: HttpStatusCode.BadRequest, message: MessagesConstants.DeleteError) :
                ResponseResult.PostResult(true, status: HttpStatusCode.OK, message: MessagesConstants.DeleteSuccess);
        }

        //public async Task<byte[]> GenerateReportAsync(FilterDirectionReportDto filter, CancellationToken cancellationToken = default)
        //{
        //    // get report file

        //    string fileDirPath = Assembly.GetExecutingAssembly().Location.Replace("Application.dll", string.Empty);
        //    Console.WriteLine(string.Format("{0}ReportsFiles\\{1}.rdlc", fileDirPath, filter.ReportName));
        //    string rdclFilePath = string.Format("{0}ReportsFiles\\{1}.rdlc", fileDirPath, filter.ReportName);

        //    // file encoding

        //    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        //    Encoding.GetEncoding("utf-8");

        //    LocalReport report = new()
        //    {
        //        ReportPath = rdclFilePath
        //    };


        //    // prepare data for report

        //    if (filter.ReportName == "DirectionReport")
        //    {
        //        IFinalResult Res = await GetByIdForEditAsync(filter.SentId);
                
        //        var data = Res.Data as EditSentDto;

        //        report.DataSources.Add(new ReportDataSource() { Name = "Direction", Value = data });
        //    }

        //    byte[] renderedBytes = report.Render(filter.ReportType);

        //    return renderedBytes;
        //}

        
    }
}
