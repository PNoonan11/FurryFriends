using FurryFriends.Data.Entities;

namespace FurryFriends.Models.Post
{
    public class PostListItem
    {
        public int Id { get; set; }
        public string Text { get; set; }

        public string UserName { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public DateTime DateTimeUpdated { get; set; }

        public int OwnerId { get; set; }
        // public CommentEntity Comments { get; set; }
    }
}