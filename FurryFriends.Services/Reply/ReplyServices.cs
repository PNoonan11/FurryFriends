using System.Security.Claims;
using System.Text.Json;
using AutoMapper;
using FurryFriends.Data;
using FurryFriends.Data.Entities;
using FurryFriends.Models.Reply;
using FurryFriends.Services.Wrapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace FurryFriends.Services.Reply
{
    public class ReplyServices : IReplyServices
    {
        private readonly int _commentId;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _DbContext;
        public ReplyServices(IHttpContextAccessor httpContextAccessor, IMapper mapper, ApplicationDbContext DbContext)
        {
            //User claims
            // var userClaims = httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;
            // var value = userClaims.FindFirst("Id")?.Value;
            // var validId = int.TryParse(value, out _commentId);
            // if (!validId)
            //     throw new Exception("Attempted to build ReplyService without User Id claim.");
            _mapper = mapper;
            _DbContext = DbContext;
        }

        public async Task<bool> CreateReplyAsync(ReplyCreate model)
        {
            // var replyEntity = _mapper.Map<ReplyCreate, ReplyEntity>(model, opt =>
            // opt.AfterMap((src, dest) => dest.CommentId = _commentId));
            var replyEntity = _mapper.Map<ReplyCreate, ReplyEntity>(model);


            _DbContext.Reply.Add(replyEntity);
            var numberOfChanges = await _DbContext.SaveChangesAsync();

            return numberOfChanges == 1;
        }

        public async Task<ReplyListItem> GetReplyByIdAsync(int replyId)
        {
            var replyToUser = await _DbContext.Reply
                .FirstOrDefaultAsync(e => e.Id == replyId);

            return replyToUser is null ? null : _mapper.Map<ReplyListItem>(replyToUser);

        }

        public async Task<IEnumerable<ReplyListItem>> GetAllReplyAsync(PaginationFilter _filter, HttpContext httpContext)
        {
            var replies = _DbContext.Reply
                .OrderBy(p => p.Id)
                .Select(entity => _mapper.Map<ReplyListItem>(entity));

            var paginationMetadata = new PaginationMetaData(replies.Count(), _filter.CurrentPage, _filter.PageSize);
            httpContext.Response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationMetadata));

            var items = await replies.Skip((_filter.CurrentPage - 1) * _filter.PageSize)
                .Take(_filter.PageSize)
                .ToListAsync();

            return replies;

        }

        public async Task<bool> UpdateReplyAsync(ReplyUpdate request)
        {
            //         var replyIsUserOwned = await _DbContext.Reply.AnyAsync(reply =>
            // reply.Id == request.Id && reply.CommentId == _commentId);
            //         if (!replyIsUserOwned)
            //             return false;
            // var updatedReply = _mapper.Map<ReplyUpdate, ReplyEntity>(request, opt =>
            //             opt.AfterMap((src, dest) => dest.CommentId = _commentId));
            var updatedReply = _mapper.Map<ReplyUpdate, ReplyEntity>(request);
            _DbContext.Entry(updatedReply).State = EntityState.Modified;
            _DbContext.Entry(updatedReply).Property(e => e.DateTimeCreated).IsModified = false;
            var numberOfChanges = await _DbContext.SaveChangesAsync();

            return numberOfChanges == 1;

        }

        public async Task<bool> DeleteAReplyAsync(int replyId)
        {
            var replyToDelete = await _DbContext.Reply.FindAsync(replyId);

            _DbContext.Reply.Remove(replyToDelete);
            return await _DbContext.SaveChangesAsync() == 1;
        }
    }
}