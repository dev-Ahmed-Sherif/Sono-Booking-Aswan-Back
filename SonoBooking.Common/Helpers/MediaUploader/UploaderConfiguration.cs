using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SonoBooking.Common.Constants;
using SonoBooking.Common.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Common.Helpers.MediaUploader
{
    public class UploaderConfiguration : IUploaderConfiguration
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly HttpRequest _request;
        public UploaderConfiguration(IWebHostEnvironment hostingEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _hostingEnvironment = hostingEnvironment;
            _request = httpContextAccessor.HttpContext.Request;
        }

        public string SaveBase64(string fileBase64, string fileName, string folderName, string oldFileName = null)
        {
            try
            {
                var path = $"{_hostingEnvironment.ContentRootPath}/{folderName}";
                if (!string.IsNullOrWhiteSpace(oldFileName)) RemoveFile(oldFileName, folderName);
                if (!fileBase64.Contains("base64")) return null;
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                var (fileExtension, data) = fileBase64.GetBase64StringContents();
                var fileFullName = $"{fileName}-{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(path, fileFullName);
                var fileBytes = Convert.FromBase64String(data);
                File.WriteAllBytes(filePath, fileBytes);
                return fileFullName;
            }
            catch (Exception)
            {
                return null;
            }

        }
        public string ConvertToBase64String(string fileName, string folderName)
        {
            var path = $"{_hostingEnvironment.ContentRootPath}/{folderName}/{fileName}";
            if (!File.Exists(path)) path = $"{_hostingEnvironment.ContentRootPath}/StaticFiles/No-Image-Available.png";
            var fileByte = File.ReadAllBytes(path);
            Stream stream = new MemoryStream(fileByte);
            var imgExtension = path.Contains("StaticFiles/No-Image-Available.png") ? ".png" : fileName.Split('.')[1];
            return $"data:{MimeTypeMap.GetMimeType(imgExtension)};base64,{Convert.ToBase64String(fileByte)}";
        }
        public Stream ConvertToStream(string fileName, string folderName)
        {
            var path = $"{_hostingEnvironment.ContentRootPath}/{folderName}/{fileName}";
            if (!File.Exists(path)) path = $"{_hostingEnvironment.ContentRootPath}/StaticFiles/No-Image-Available.png";
            var fileByte = File.ReadAllBytes(path);
            return new MemoryStream(fileByte);
        }
        public void RemoveFile(string fileName, string folderName)
        {
            var fullPath = $"{_hostingEnvironment.ContentRootPath}/{folderName}/{fileName}";
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
        
        public void DeleteFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) return;
            var fullPath = Path.Combine(_hostingEnvironment.ContentRootPath, "wwwroot", filePath);
            if (File.Exists(fullPath)) File.Delete(fullPath);
        }

        public async Task<string> UploadFile(IFormFile file, string subDirectory = FoldersName.General, CancellationToken cancellationToken = default)
        {
            if (file == null) return "";

            bool hasSubDirectory = !string.IsNullOrWhiteSpace(subDirectory);
            string uniqueFileName;
            string filePath;

            if (file.Length < 0) throw new NullReferenceException();

            if (file.Length > 5 * 1024 * 1024) return "Size";

            string[] allowedFileExtensions = [".jpg", ".jpeg", ".png", ".webp", ".gif", ".bmp", ".svg", ".ico", ".tif", ".tiff", ".avif", ".heic", ".heif"];

            // Generate a unique file name to avoid naming conflicts
            string fileExtension = Path.GetExtension(file.FileName);
            uniqueFileName = $"{DateTime.Now.Ticks}{fileExtension}";

            // Filter file according to its Extension
            if (allowedFileExtensions.Contains(fileExtension.ToLower()))
            {
                // Set the path where the file will be stored
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                if (hasSubDirectory)
                {
                    uploadsFolder = Path.Combine(uploadsFolder, subDirectory);
                }
                filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Create the directory if it doesn't exist
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Save the file to the specified path
                using var fileStream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(fileStream, cancellationToken);
            }
            else
            {
                return "Type";
            }
            return hasSubDirectory ? $"{subDirectory}/{uniqueFileName}" : uniqueFileName;

        }
        public string GetFileUrl(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) return string.Empty;
            return $"{_request.Scheme}://{_request.Host.Value}/{filePath}";
        }
    }
}

