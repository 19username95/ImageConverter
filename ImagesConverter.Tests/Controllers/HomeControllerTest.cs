using ImagesConverter.Controllers;
using ImagesConverter.Models;
using ImagesConverter.Services;
using ImagesConverter.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI.WebControls;

namespace ImagesConverter.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        private static int id;

        [TestMethod]
        public async Task AddImage()
        {
            HttpPostedFileBaseMock fileBaseMock;  // Get the FileUpload object.
            using (FileStream fs = File.OpenRead(@"../../Assets/file.png"))
            {
                fileBaseMock = new HttpPostedFileBaseMock(fs, "image/png", "file.png");
                fs.Flush();

                HomeController controller = new HomeController();

                var result = await controller.SaveImage(new ViewModels.ImageModel() { Title = "test" }, fileBaseMock);

                Assert.IsTrue(result.Id > 0);

                ImagesDbService imagesDbService = new ImagesDbService(new StandardApiDbContext());

                var imgs = await imagesDbService.LoadImagesAsync(new List<int>() { result.Id });

                Assert.IsTrue(imgs.Count > 0);
                Assert.AreEqual(imgs[0].Id, result.Id);

                // Save result for download test
                id = result.Id;
            }

        }

        [TestMethod]
        public async Task AddImage_Error()
        {
            HttpPostedFileBaseMock fileBaseMock;  // Get the FileUpload object.
            using (var fs = new MemoryStream())
            {
                fileBaseMock = new HttpPostedFileBaseMock(fs, "image/png", "file.png");
                fs.Flush();

                HomeController controller = new HomeController();

                var model = new ViewModels.ImageModel() { Title = "test" };
                var result = await controller.SaveImage(model, fileBaseMock);

                Assert.IsNull(result);
                Assert.AreEqual(model.StatusMessage, "Error occured while processing the file");
            }
        }


        [TestMethod]
        public async Task GetZipWithBase64s()
        {
            HomeController controller = new HomeController();
            var result = await controller.GetZipWithBase64s(id.ToString());

            Assert.AreEqual((result as FileContentResult).ContentType, "application/zip");
            Assert.IsNotNull((result as FileContentResult).FileContents);
            Assert.IsTrue((result as FileContentResult).FileContents.Length > 0);
        }
    }
}
