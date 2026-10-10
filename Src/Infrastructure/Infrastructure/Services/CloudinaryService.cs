using Application.Interfaces.Services;
using Application.ResponseDTO;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace Infrastructure.Services;

public sealed class CloudinaryService : IImageStorageService
{
    private readonly Cloudinary _cloudinary;
    public CloudinaryService(Cloudinary cloudinary)
    {
        _cloudinary = cloudinary;
    }

    public async Task DeleteAsync(string publicId, CancellationToken ct)
    {
        var deleteParams = new DeletionParams(publicId);

        var result = await _cloudinary.DestroyAsync(deleteParams);
        if (result.Error is not null)
            throw new InvalidOperationException(result.Error.Message);
    }

    public async Task<UploadedImageResult> UploadAsync(
        Stream imageStream,
        string fileName,
        string contentType,
        CancellationToken ct)
    {
        var UploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, imageStream),
            Folder = "SupportFlow/Images",
            UseFilename = true,
            UniqueFilename = true,
            Overwrite = false
        };

        var result = await _cloudinary.UploadAsync(UploadParams, ct);
        if (result.Error is not null)
            throw new InvalidOperationException(result.Error.Message);

        return new UploadedImageResult(
            result.SecureUrl.ToString(),
             result.PublicId);
    }
}
