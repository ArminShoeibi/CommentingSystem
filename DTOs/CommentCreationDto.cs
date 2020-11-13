using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CommentingSystem.DTOs
{
    public class CommentCreationDto
    {

        [Required]
        [StringLength(1000, MinimumLength = 10)]
        public string Content { get; set; }

        [EmailAddress]
        [StringLength(100, MinimumLength = 6)]
        [Required]
        public string Email { get; set; }


        [StringLength(60, MinimumLength = 3)]
        [Required]
        public string FullName { get; set; }
    }
}
