namespace Blog.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity;

    public partial class User
    {
        public int UserId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "The username must be between 5 and 50 characers")]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "The password must be at least 6 characters")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public string PasswordSalt { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public System.DateTime DateCreated { get; set; }

        public virtual ICollection<Post> Post { get; set; }

        public virtual ICollection<Comment> Comment { get; set; }
    }

    public class UserDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
    }

    public partial class RegisterUser : User
    {
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password doesn\'t match. Please Enter again")]
        [Display(Name = "Confirm password")]
        public string PasswordConfirm { get; set; }

    }

    public partial class LoginUser
    {
        [Required]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "The username must be between 5 and 50 characers")]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "The password must be at least 6 characters")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

    }
}
