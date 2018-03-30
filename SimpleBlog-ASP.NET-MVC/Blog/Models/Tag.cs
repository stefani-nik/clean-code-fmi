//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Blog.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public partial class Tag
    {
        public Tag()
        {
            this.Post = new HashSet<Post>();
        }

        public int TagId { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "The tag name must be between 3 nad 20 characters")]
        public string TagName { get; set; }

        public virtual ICollection<Post> Post { get; set; }
    }
}
