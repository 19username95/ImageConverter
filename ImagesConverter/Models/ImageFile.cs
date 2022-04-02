using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ImagesConverter.Models
{
    public class ImageFile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Title { get; set; }
        
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string ImageName { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string MimeType { get; set; }

        [Required]
        public string ImageBase64 { get; set; }

        [Required]
        public DateTime DateConverted { get; set; }
    }
}