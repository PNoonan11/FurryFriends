using System.Security.Claims;
using System.Text.Json;
using AutoMapper;
using FurryFriends.Data;
using FurryFriends.Data.Entities;
using FurryFriends.Models.Post;
using FurryFriends.Services.Wrapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace FurryFriends.Services.Post
{
    public class PostServices : IPostServices
    {
        private readonly int _userId;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _DbContext;
        public PostServices(IHttpContextAccessor httpContextAccessor, IMapper mapper, ApplicationDbContext DbContext)
        {
            // var userClaims = httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;
            // var value = userClaims.FindFirst("Id")?.Value;
            // var validId = int.TryParse(value, out _userId);
            // if (!validId)
            //     throw new Exception("Attempted to build PostService without User Id claim.");
            _mapper = mapper;
            _DbContext = DbContext;
        }

        public async Task<bool> CreatePostAsync(PostCreate model)
        {
            // var postEntity = _mapper.Map<PostCreate, PostEntity>(model, opt =>
            // opt.AfterMap((src, dest) => dest.OwnerId = _userId));
            var postEntity = _mapper.Map<PostCreate, PostEntity>(model);


            _DbContext.Post.Add(postEntity);
            var numberOfChanges = await _DbContext.SaveChangesAsync();

            return numberOfChanges == 1;
        }

        public async Task<PostListItem> GetPostByIdAsync(int postId)
        {
            var postToUser = await _DbContext.Post
            // .Include(r => r.Comments)
            .FirstOrDefaultAsync(e => e.Id == postId);

            return postToUser is null ? null : _mapper.Map<PostListItem>(postToUser);

        }

        public async Task<IEnumerable<PostListItem>> GetAllPostsAsync(PaginationFilter _filter, HttpContext httpContext)
        {
            var posts = _DbContext.Post
                .OrderBy(p => p.Id)
                .Select(entity => _mapper.Map<PostListItem>(entity));

            var paginationMetadata = new PaginationMetaData(posts.Count(), _filter.CurrentPage, _filter.PageSize);
            httpContext.Response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationMetadata));

            var items = await posts.Skip((_filter.CurrentPage - 1) * _filter.PageSize)
                .Take(_filter.PageSize)
                .ToListAsync();

            return items;

        }

        public async Task<bool> UpdatePostAsync(PostUpdate request)
        {
            var postIsUserOwned = await _DbContext.Post.AnyAsync(post =>
    post.Id == request.Id);
            if (!postIsUserOwned)
                return false;

            var newPost = _mapper.Map<PostUpdate, PostEntity>(request);
            _DbContext.Entry(newPost).State = EntityState.Modified;
            _DbContext.Entry(newPost).Property(e => e.DateTimeCreated).IsModified = false;
            var numberOfChanges = await _DbContext.SaveChangesAsync();

            return numberOfChanges == 1;

        }

        public async Task<bool> DeleteAPostAsync(int postId)
        {
            var postToDelete = await _DbContext.Post.FindAsync(postId);

            _DbContext.Post.Remove(postToDelete);
            return await _DbContext.SaveChangesAsync() == 1;
        }


    }
}