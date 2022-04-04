using ImagesConverter.Helpers;
using ImagesConverter.JsonModels;
using ImagesConverter.Models;
using ImagesConverter.Services;
using ImagesConverter.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ImagesConverter.Controllers
{
    public class HomeController : Controller
    {
        private readonly ImagesDbService _dbService;

        public HomeController()
            :base()
        {
            this._dbService = new ImagesDbService(new StandardApiDbContext());
        }

        public async Task<ActionResult> Index()
        {
            var imageFiles = await this._dbService.LoadImagesAsync();
            return View(new IndexViewModel(imageFiles));
        }
        
        private ImageFile MapImageToImageFile(ImageModel model, HttpPostedFileBase image)
        {
            var result = new ImageFile()
            {
                DateConverted = DateTime.Now,
                ImageName = image.FileName,
                MimeType = image.ContentType,
                Title = model.Title
            };

            byte[] imageAsBytes = new byte[image.ContentLength];
            using (BinaryReader binaryReader = new BinaryReader(image.InputStream))
            {
                imageAsBytes = binaryReader.ReadBytes(image.ContentLength);
            }
            string imageAsBase64 = Convert.ToBase64String(imageAsBytes);

            result.ImageBase64 = imageAsBase64;

            return result;
        }
        
        public async Task<ImageFile> SaveImage(ImageModel model, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (file != null)
                    {
                        if (!file.ContentType.Contains("image"))
                        {
                            model.SetFileError("File is not an image");
                        }
                        else
                        {
                            var entity = MapImageToImageFile(model, file);
                            var result = await this._dbService.AddImageAsync(entity);
                            model.Success();
                            return result;
                        }
                    }
                    else
                    {
                        model.SetFileError("File is required!");
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    model.SetFileError("Error occured while processing the file");
                }
            }

            return null;
        }

        private async Task<Dictionary<string, string>> RenderUploadForm(ImageModel model)
        {
            var viewData = new Dictionary<string, string>();

            viewData.Add("uploadFormWrapper", RenderHelper.PartialView(this.ControllerContext, IndexViewModel.PARTIAL_UPLOAD_FORM, model));

            if (model.Status.GetValueOrDefault())
            {
                var imageFiles = await this._dbService.LoadImagesAsync();
                viewData.Add("imagesListWrapper", RenderHelper.PartialView(this.ControllerContext, IndexViewModel.PARTIAL_IMAGES_LIST, new ImageListModel(imageFiles)));
            }
            return viewData;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> AddImage(ImageModel model, HttpPostedFileBase file)
        {
            await SaveImage(model, file);

            var viewData = await RenderUploadForm(model);

            return Json(JsonResponse.Ok(viewData));
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> GetZipWithBase64s(string ids)
        {
            var imageIDs = (ids ?? String.Empty)
                .Split('-')
                .Where(_ => Int32.TryParse(_, out var __))
                .Select(_ => Int32.Parse(_))
                .Distinct()
                .ToList();

            if (imageIDs == null || imageIDs.Count == 0)
            {
                return HttpNotFound();
            }

            IEnumerable<ImageFile> files = await this._dbService.LoadImagesAsync(imageIDs);

            var fileNamesCheck = new Dictionary<string, int>();

            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var f in files)
                    {
                        var fileName = f.ImageName;

                        if (!fileNamesCheck.ContainsKey(fileName))
                        {
                            fileNamesCheck.Add(fileName, 1);
                        }
                        else
                        {
                            var nextIndex = fileNamesCheck[fileName]++;
                            fileNamesCheck[fileName] = nextIndex;
                            fileName = $"{Path.GetFileNameWithoutExtension(fileName)}-{nextIndex}{Path.GetExtension(fileName)}";
                        }

                        var txtFile = archive.CreateEntry($"{Path.GetFileNameWithoutExtension(fileName)}.txt");
                        using (var entryStream = txtFile.Open())
                        {
                            using (var streamWriter = new StreamWriter(entryStream))
                            {
                                streamWriter.Write(f.ImageBase64);
                            }
                        }
                        var imageFile = archive.CreateEntry(fileName);
                        byte[] byteContent = Convert.FromBase64String(f.ImageBase64);
                        using (var entryStream = imageFile.Open())
                        {
                            foreach(var b in byteContent)
                            {
                                entryStream.WriteByte(b);
                            }
                        }
                    }

                }
                return File(memoryStream.ToArray(), "application/zip", "Export_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".zip");
            }
        }
    }
}