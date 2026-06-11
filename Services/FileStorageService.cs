using System.Security.Cryptography;

namespace Cms0053Demo.Services;

public class FileStorageService(IWebHostEnvironment env)
{
    private string UploadsDir => Path.Combine(env.WebRootPath, "uploads");

    public async Task<(string storedFileName, long sizeBytes, string sha256Hex)> SaveAsync(IFormFile file)
    {
        Directory.CreateDirectory(UploadsDir);
        var ext = Path.GetExtension(file.FileName);
        var guid = Guid.NewGuid().ToString("N");
        var storedName = $"{guid}{ext}";
        var fullPath = Path.Combine(UploadsDir, storedName);

        await using var fs = File.Create(fullPath);
        await file.CopyToAsync(fs);

        var bytes = await File.ReadAllBytesAsync(fullPath);
        var hash = Convert.ToHexString(SHA256.HashData(bytes)).ToLower();

        return (storedName, file.Length, hash);
    }

    public async Task<(string storedFileName, long sizeBytes, string sha256Hex)> SaveTextAsync(string content, string extension)
    {
        Directory.CreateDirectory(UploadsDir);
        var guid = Guid.NewGuid().ToString("N");
        var storedName = $"{guid}{extension}";
        var fullPath = Path.Combine(UploadsDir, storedName);

        await File.WriteAllTextAsync(fullPath, content);

        var bytes = System.Text.Encoding.UTF8.GetBytes(content);
        var hash = Convert.ToHexString(SHA256.HashData(bytes)).ToLower();

        return (storedName, bytes.Length, hash);
    }

    public string ReadText(string storedFileName)
    {
        var path = Path.Combine(UploadsDir, storedFileName);
        return File.Exists(path) ? File.ReadAllText(path) : "";
    }

    public string SyntheaSamplePath(string fileName) =>
        Path.Combine(env.WebRootPath, "synthea-samples", fileName);

    public string ReadSyntheaSample(string fileName) =>
        File.ReadAllText(SyntheaSamplePath(fileName));
}
