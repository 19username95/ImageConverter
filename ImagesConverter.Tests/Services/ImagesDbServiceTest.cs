using ImagesConverter.Models;
using ImagesConverter.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ImagesConverter.Tests.Services
{
    [TestClass]
    public class ImagesDbServiceTest
    {
        private static ImagesDbService imageDbService = new ImagesDbService(new StandardApiDbContext());
        private static int id;

        [TestMethod]
        public async Task AddImageAsync()
        {
            using (FileStream fs = File.OpenRead(@"../../Assets/file.png"))
            {
                var file = new ImageFile()
                {
                    DateConverted = DateTime.Now,
                    ImageName = "file.png",
                    MimeType = "image/png",
                    Title = "Test title"
                };

                byte[] imageAsBytes = new byte[fs.Length];
                using (BinaryReader binaryReader = new BinaryReader(fs))
                {
                    imageAsBytes = binaryReader.ReadBytes(Convert.ToInt32(fs.Length));
                }
                string imageAsBase64 = Convert.ToBase64String(imageAsBytes);

                file.ImageBase64 = imageAsBase64;

                var result = await imageDbService.AddImageAsync(file);

                Assert.IsTrue(result.Id > 0);

                id = result.Id;
            }
        }

        [TestMethod]
        public async Task LoadImagesAsync()
        {
            var result = await imageDbService.LoadImagesAsync();
            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod]
        public async Task LoadImagesAsyncById()
        {
            var result = await imageDbService.LoadImagesAsync(new List<int>() { id });
            Assert.IsTrue(result.Count > 0);
            Assert.AreEqual(result[0].Id, id);
        }
    }
}
