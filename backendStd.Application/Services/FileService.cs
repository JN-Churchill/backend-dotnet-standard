using backendStd.Core.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using IWebHostEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace backendStd.Application.Services;

/// <summary>
/// 文件上传服务
/// </summary>
public class FileService
{
    private readonly FileUploadOptions _options;
    private readonly IWebHostEnvironment _environment;

    public FileService(
        IOptions<FileUploadOptions> options,
        IWebHostEnvironment environment)
    {
        _options = options.Value;
        _environment = environment;
    }

    /// <summary>
    /// 上传单个文件
    /// </summary>
    public async Task<string> UploadAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new Common.Exceptions.BusinessException("文件不能为空");

        // 验证文件大小
        if (file.Length > _options.MaxSize)
            throw new Common.Exceptions.BusinessException($"文件大小不能超过{_options.MaxSize / 1024 / 1024}MB");

        // 验证文件扩展名
        var extension = Path.GetExtension(file.FileName).ToLower();
        if (!_options.AllowedExtensions.Contains(extension))
            throw new Common.Exceptions.BusinessException($"不支持的文件格式：{extension}");

        // 生成文件名（使用雪花ID）
        var fileName = $"{Yitter.IdGenerator.YitIdHelper.NextId()}{extension}";

        // 按日期创建子目录
        var dateFolder = DateTime.Now.ToString("yyyyMMdd");
        var uploadPath = Path.Combine(_environment.WebRootPath, _options.UploadPath, dateFolder);

        // 确保目录存在
        if (!Directory.Exists(uploadPath))
            Directory.CreateDirectory(uploadPath);

        // 保存文件
        var filePath = Path.Combine(uploadPath, fileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // 返回相对路径
        return $"/{_options.UploadPath}/{dateFolder}/{fileName}";
    }

    /// <summary>
    /// 上传多个文件
    /// </summary>
    public async Task<List<string>> UploadMultipleAsync(List<IFormFile> files)
    {
        var result = new List<string>();

        foreach (var file in files)
        {
            var path = await UploadAsync(file);
            result.Add(path);
        }

        return result;
    }

    /// <summary>
    /// 删除文件
    /// </summary>
    public Task<bool> DeleteAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return Task.FromResult(false);

        try
        {
            // 转换为物理路径
            var physicalPath = Path.Combine(_environment.WebRootPath, filePath.TrimStart('/'));
            
            if (File.Exists(physicalPath))
            {
                File.Delete(physicalPath);
                return Task.FromResult(true);
            }
        }
        catch
        {
            // 忽略删除错误
        }

        return Task.FromResult(false);
    }
}
