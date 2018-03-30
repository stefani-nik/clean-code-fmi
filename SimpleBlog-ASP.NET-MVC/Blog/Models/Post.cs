namespace Blog.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public partial class Post
    {
        public int PostId { get; set; }

        public Nullable<int> UserId { get; set; }

        [Required]
        [StringLength(80, MinimumLength = 4, ErrorMessage = "The title must be between 8 and 40 characters.")]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [StringLength(150, MinimumLength = 4, ErrorMessage = "The about content must be between 4 and 150 characters.")]
        public string About { get; set; }

        [Required]
        [StringLength(int.MaxValue, MinimumLength = 10, ErrorMessage = "The content is too short.")]
        public string Content { get; set; }

        [DataType(DataType.Date)]
        public System.DateTime DatePosted { get; set; }
        public Nullable<int> CategoryId { get; set; }

        public virtual User User { get; set; }

        public virtual Category Category { get; set; }

        public virtual ICollection<Comment> Comment { get; set; }

        public virtual ICollection<Tag> Tag { get; set; }

        public int[] SelectedTags { get; set; }
    }
}
