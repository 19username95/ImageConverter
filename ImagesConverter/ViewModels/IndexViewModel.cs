using ImagesConverter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImagesConverter.ViewModels
{
    public class IndexViewModel
    {
        public const string PARTIAL_UPLOAD_FORM = "~/Views/Shared/Partials/UploadForm.cshtml";
        public const string PARTIAL_IMAGES_LIST = "~/Views/Shared/Partials/ImagesList.cshtml";

        public ImageModel ImageModel { get; set; }

        public ImageListModel ImagesList { get; set; }

        public IndexViewModel()
        {
            this.ImageModel = new ImageModel();
            this.ImagesList = new ImageListModel();
        }

        public IndexViewModel(IEnumerable<ImageFile> entities)
            :this()
        {
            this.ImagesList = new ImageListModel(entities);
        }
    }
}