using ImagesConverter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImagesConverter.ViewModels
{
    public class ImageListModel
    {
        public List<ImageItemModel> Items { get; set; }

        public ImageListModel()
        {
            this.Items = new List<ImageItemModel>();
        }

        public ImageListModel(IEnumerable<ImageFile> entities)
            :this()
        {
            this.Items = entities.Select(_ => new ImageItemModel(_)).ToList();
        }
    }
}