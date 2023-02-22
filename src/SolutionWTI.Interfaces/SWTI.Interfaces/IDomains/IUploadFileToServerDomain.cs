using Microsoft.AspNetCore.Http;
using SWTI.Enums;
using SWTI.Utils;

namespace SWTI.Interfaces.IDomains
{
    public interface IUploadFileToServerDomain
    {
        (string, BaseResponse) UploadFileToServer(IFormFile file, FolderUploadEnum type, string key, CancellationToken cancellationToken);
        (string, BaseResponse) DeleteAndUploadFileToServer(IFormFile file, FolderUploadEnum type, string key, string keyOld, CancellationToken cancellationToken);
    }
}
