using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ImagesConverter.ViewModels
{
    public class ImageModel
    {
        [Required]
        [Display(Name = "Image title")]
        [MaxLength(100, ErrorMessage = "Title can be maximum 100 characters")]
        public string Title { get; set; }

        public bool? Status { get; set; }

        public string StatusMessage { get; set; }

        public void SetFileError(string message)
        {
            this.Status = false;
            this.StatusMessage = message;
        }

        public void Success()
        {
            this.Status = true;
            this.StatusMessage = "File uploaded successfully";
        }

        public void Reset()
        {
            this.Status = null;
            this.StatusMessage = null;
        }
    }
}