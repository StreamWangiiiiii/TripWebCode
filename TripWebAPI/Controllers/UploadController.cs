using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using TripWebAPI.Filters;
using Microsoft.Net.Http.Headers;
using TripWebData;

namespace TripWebAPI.Controllers
{
    /// <summary>
    /// 文件上传
    /// </summary>
    [ApiController]
    [Route("[controller]/[action]")]
    [AuthorizeAttribute]
    public class UploadController : BaseController
    {
        private readonly IWebHostEnvironment _hostEnvironment;

        /// <summary>
        /// 注入服务
        /// </summary>
        /// <param name="hostEnvironment"></param>
        public UploadController(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        /// <summary>
        /// 上传文件（支持多文件/大文件500M）
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 609715200)]
        [RequestSizeLimit(609715200)]
        public async Task<Results<List<string>>> UploadFile()
        {
            var request = HttpContext.Request;

            if (!request.HasFormContentType ||
                !MediaTypeHeaderValue.TryParse(request.ContentType, out var mediaTypeHeader) ||
                string.IsNullOrEmpty(mediaTypeHeader.Boundary.Value))
            {
                return Results<List<string>>.FailResult("文件类型不支持");
            }
            var reader = new MultipartReader(mediaTypeHeader.Boundary.Value,
            request.Body);
            var section = await reader.ReadNextSectionAsync();
            List<string> serverFilePathList = new();

            while (section != null)
            {
                var hasContentDispositionHeader =
                ContentDispositionHeaderValue.TryParse(section.ContentDisposition,out var contentDisposition);
                if (hasContentDispositionHeader &&
                contentDisposition!.DispositionType.Equals("form-data") &&
                !string.IsNullOrEmpty(contentDisposition.FileName.Value))
                {
                    // 获取文件后缀名
                    var extension =
                    Path.GetExtension(contentDisposition.FileName.Value);
                    // 为文件重命名，防止文件重名
                    var fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") +
                    extension;
                    // 文件保存的文件夹路径
                    var uploadPath = Path.Combine(_hostEnvironment.WebRootPath,
                    "upload");
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }
                    var fileFullPath = Path.Combine(uploadPath, fileName);
                    try
                    {
                        using var targetStream = System.IO.File.Create(fileFullPath);
                        await section.Body.CopyToAsync(targetStream);
                        serverFilePathList.Add("/upload/" + fileName);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                section = await reader.ReadNextSectionAsync();
            }
            return Results<List<string>>.DataResult(serverFilePathList);
        }
    }
}
