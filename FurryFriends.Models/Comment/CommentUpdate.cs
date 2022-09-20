using FurryFriends.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace FurryFriends.Models.Comment
{
    public class CommentUpdate
    {

        public int Id { get; set; }

        public string Text { get; set; }

        public string UserName { get; set; }

    }
}