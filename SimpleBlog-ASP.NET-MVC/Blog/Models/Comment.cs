using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Blog.Models
{
    public class Comment
    {
        public int CommentId { get; set; }

        public Nullable<int> UserId { get; set; }

        public Nullable<int> PostId { get; set; }

        [Required]
        [StringLength(int.MaxValue, MinimumLength = 10, ErrorMessage = "The comment is too short")]
        public string Content { get; set; }

        public System.DateTime DatePublished { get; set; }

        public virtual User User { get; set; }

        public virtual Post Post { get; set; }

    }
}