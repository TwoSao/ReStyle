namespace ReStyle.Application.Interfaces;

public interface IImageService
{
    Task<string?> SaveImageAsync(Stream imageStream, string fileName);
    void DeleteImage(string? imagePath);
}
