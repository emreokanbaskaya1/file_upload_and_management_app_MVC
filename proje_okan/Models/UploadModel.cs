using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace proje_okan.Models
{
    public class UploadModel
    {
        [Required]
        [Display(Name = "file")]
        public IFormFile File { get; set; }
    }
}
