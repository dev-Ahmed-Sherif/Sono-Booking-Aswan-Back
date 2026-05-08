using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace SonoBooking.Common.Helpers.MediaUploader
{
    public interface IUploaderConfiguration
    {
        string SaveBase64(string fileBase64, string fileName, string folderName, string oldFileName = null);
        string ConvertToBase64String(string fileName, string folderName);
        Stream ConvertToStream(string fileName, string folderName);
        void RemoveFile(string fileName, string folderName);
        string GetFileUrl(string filePath);
        Task<string> UploadFile(IFormFile file, string subDirectory = "General", CancellationToken cancellationToken = default);
        void DeleteFile(string filePath);
    }
}
