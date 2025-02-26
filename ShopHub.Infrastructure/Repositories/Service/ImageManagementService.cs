using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using ShopHub.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopHub.Infrastructure.Repositories.Service
{
    public class ImageManagementService : IImageManagementService
    {
        private readonly IFileProvider fileProvider;
        public ImageManagementService(IFileProvider fileProvider)
        {
            this.fileProvider = fileProvider;
        }
        public async Task<List<string>> AddImageAsync(IFormFileCollection files, string src)
        {
            var saveImageSrc = new List<string>();
            var imageDirectory = Path.Combine("wwwroot", "Images", src);
            if (Directory.Exists(imageDirectory) is not true)
            {
                Directory.CreateDirectory(imageDirectory);
            }
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    // get image name
                    var imageName = file.FileName;
                    // get image path
                    var imageSrc = $"Images/{src}/{imageName}";


                    var root = Path.Combine(imageDirectory, imageName);
                   
                    using (FileStream stream = new FileStream(root, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    saveImageSrc.Add(imageSrc);
                }
            }
            return saveImageSrc;
        }

        public void DeleteImageAsync(string src)
        {
            var info = fileProvider.GetFileInfo(src);
            var root = info.PhysicalPath;
            File.Delete(root);
        }
    }
}
