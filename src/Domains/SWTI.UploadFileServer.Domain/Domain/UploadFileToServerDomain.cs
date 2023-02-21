using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SWTI.Configurations;
using SWTI.Enums;
using SWTI.Interfaces.IDomains;
using SWTI.Utils;

namespace SWTI.UploadFileServer.Domain.Domain
{
    public class UploadFileToServerDomain : IUploadFileToServerDomain
    {
        private readonly ILogger<UploadFileToServerDomain> _logger;
        private readonly IHostingEnvironment _environment;
        private readonly FolderImageConfig _folderConfig;
        private static readonly string[] _validExtensions = { ".jpg", ".bmp", ".gif", ".png" }; //  etc

        public UploadFileToServerDomain(IHostingEnvironment hostingEnvironment
            , IOptions<FolderImageConfig> optionns
            , ILogger<UploadFileToServerDomain> logger)
        {
            _logger = logger;
            _environment = hostingEnvironment;
            _folderConfig = optionns.Value;
        }

        public (string, BaseResponse?) UploadFileToServer(IFormFile file, FolderUploadEnum type, string key, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"UploadFileToServerDomain >> UploadFileToServer >> {file.FileName} , {type}");

            if (CheckExtensionImage(file.FileName) == false)
            {
                _logger.LogError($"UploadFileToServerDomain >> UploadFileToServer >> CheckExtensionImage fail");
                return (string.Empty, BaseResponseExt.Error(400, "Extension file invalid!"));
            }

            string pathFolder = Path.Combine(_environment.WebRootPath, GetFolder(type));
            if (!Directory.Exists(pathFolder))
            {
                Directory.CreateDirectory(pathFolder);
            }

            var fileName = key + file.FileName;
            var pathFile = Path.Combine(pathFolder, fileName);



            using var stream = new FileStream(pathFile, FileMode.Create);
            file.CopyTo(stream);
            _logger.LogInformation($"UploadFileToServerDomain >> UploadFileToServer >> {file.FileName} , {type}");

            return (Path.Combine("wwwroot", fileName), null);
        }

        public string DeleteIfExistFileServer(string pathFile, CancellationToken cancellationToken)
        {
            var wwwPath = this._environment.WebRootPath;
            var contentPath = this._environment.ContentRootPath;

            string path = Path.Combine(wwwPath, "Uploads");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return "";
        }

        public bool CheckExtensionImage(string fileName)
        {
            return _validExtensions.Contains(General.GetFileExtensionFromUrl(fileName).ToLower());
        }

        public string GetFolder(FolderUploadEnum type)
        {
            switch (type)
            {
                case FolderUploadEnum.Partner:
                    {
                        return _folderConfig.FolderPartner;
                    }

                case FolderUploadEnum.Product:
                    {
                        return _folderConfig.FolderProduct;
                    }

                default:
                    {
                        return String.Empty;
                    }
            }
        }
    }
}
