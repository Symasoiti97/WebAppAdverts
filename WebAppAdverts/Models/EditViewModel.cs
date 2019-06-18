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
        public byte[] ImageByte { get; set; }
        public IFormFile Image { get; set; }
    }
}
