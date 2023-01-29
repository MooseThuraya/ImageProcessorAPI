using Microsoft.AspNetCore.Http;

namespace SimpleImageProcessor.Models
{
    public class FileUpload
    {
        public IFormFile file { get; set; }
    }
}
