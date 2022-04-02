using ImagesConverter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImagesConverter.ViewModels
{
    public class ImageItemModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string ImageName { get; set; }

        public string MimeType { get; set; }

        public string ImageBase64 { get; set; }

        public DateTime DateConverted { get; set; }

        public ImageItemModel()
        {

        }

        public ImageItemModel(ImageFile entity)
            :this()
        {
            this.Id = entity.Id;
            this.Title = entity.Title;
            this.ImageName = entity.ImageName;
            this.MimeType = entity.MimeType;
            this.ImageBase64 = entity.ImageBase64;
            this.DateConverted = entity.DateConverted;
        }
    }
}