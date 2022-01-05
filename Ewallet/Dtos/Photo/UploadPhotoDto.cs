using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ewallet.Dtos.Photo
{
    public class UploadPhotoDto
    {
        [Required]
        public IFormFile Photo { get; set; }
        public string PublicId { get; set; }
        public string Url { get; set; }
    }
}
