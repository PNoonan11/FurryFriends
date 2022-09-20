using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurryFriends.Data.Entities
{
    public class CommentEntity
    {
        [Key]
        public int Id { get; set; }
        public string Text { get; set; }
        public string UserName { get; set; }
        public DateTime DateTimeCreated { get; set; }
        [ForeignKey("RelatedPost")]
        public int PostId { get; set; }
        //public PostEntity RelatedPost { get; set; }
    }
}