using System.Security.Claims;
using AutoMapper;
using FurryFriends.Data;
using FurryFriends.Data.Entities;
using FurryFriends.Models.Post;
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
            var userClaims = httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;
            var value = userClaims.FindFirst("Id")?.Value;
            var validId = int.TryParse(value, out _userId);
            if (!validId)
                throw new Exception("Attempted to build PostService without User Id claim.");
            _mapper = mapper;
            _DbContext = DbContext;
        }

        public async Task<bool> CreatePostAsync(PostCreate model)
        {
            var postEntity = _mapper.Map<PostCreate, PostEntity>(model, opt =>
            opt.AfterMap((src, dest) => dest.OwnerId = _userId));


            _DbContext.Post.Add(postEntity);
            var numberOfChanges = await _DbContext.SaveChangesAsync();

            return numberOfChanges == 1;
        }

        public async Task<PostListItem> GetPostByIdAsync(int postId)
        {
            var postToUser = await _DbContext.Post
                .FirstOrDefaultAsync(e => e.Id == postId && e.OwnerId == _userId);

            return postToUser is null ? null : _mapper.Map<PostListItem>(postToUser);

        }

        public async Task<IEnumerable<PostListItem>> GetAllPostsAsync()
        {
            var posts = _DbContext.Post
                .Where(entity => entity.OwnerId == _userId)
                .OrderBy(p => p.Id)
                .Select(entity => _mapper.Map<PostListItem>(entity));

            return posts;

        }

        public async Task<bool> UpdatePostAsync(PostUpdate request)
        {
            var postIsUserOwned = await _DbContext.Post.AnyAsync(note =>
    note.Id == request.Id && note.OwnerId == _userId);
            if (!postIsUserOwned)
                return false;

            var newPost = _mapper.Map<PostUpdate, PostEntity>(request, opt =>
            opt.AfterMap((src, dest) => dest.OwnerId = _userId));
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