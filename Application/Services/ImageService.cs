using ReStyle.Application.Interfaces;

namespace ReStyle.Application.Services;

/// <summary>
/// Saves images to local app data directory and returns relative path.
/// </summary>
public class ImageService : IImageService
{
    private readonly string _imagesDir;

    public ImageService()
    {
        _imagesDir = Path.Combine(FileSystem.AppDataDirectory, "images");
        Directory.CreateDirectory(_imagesDir);
    }

    public async Task<string?> SaveImageAsync(Stream imageStream, string fileName)
    {
        try
        {
            var ext = Path.GetExtension(fileName).ToLower();
            if (ext is not (".jpg" or ".jpeg" or ".png" or ".webp"))
                return null;

            var uniqueName = $"{Guid.NewGuid()}{ext}";
            var fullPath = Path.Combine(_imagesDir, uniqueName);

            await using var fileStream = File.Create(fullPath);
            await imageStream.CopyToAsync(fileStream);
            return fullPath;
        }
        catch
        {
            return null;
        }
    }

    public void DeleteImage(string? imagePath)
    {
        if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
            File.Delete(imagePath);
    }
}
