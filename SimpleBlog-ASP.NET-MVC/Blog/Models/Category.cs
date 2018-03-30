namespace Blog.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public partial class Category
    {
        public Category()
        {
            this.Post = new HashSet<Post>();
        }

        public int CategoryId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "The category name must be between 5 and 50 characters")]
        public string Content { get; set; }

        public virtual ICollection<Post> Post { get; set; }
    }
}
