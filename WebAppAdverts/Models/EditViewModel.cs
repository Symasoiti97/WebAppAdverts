using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace WebAppAdverts.Models
{
    public class EditViewModel
    {
        public Guid AdvertId { get; set; }
        [Required]
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public IFormFile Image { get; set; }
    }
}
