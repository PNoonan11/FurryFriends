using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurryFriends.Data.Entities
{
    public class PostEntity
    {
        [Key]
        public int Id { get; set; }
        public string Text { get; set; }
        public string UserName { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public DateTime DateTimeUpdated { get; set; }

        public int OwnerId { get; set; }
        public UserEntity Owner { get; set; }
        //public CommentEntity Comments { get; set; }
    }
}