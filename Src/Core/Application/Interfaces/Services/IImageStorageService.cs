using Application.ResponseDTO;

namespace Application.Interfaces.Services
{
    public interface IImageStorageService
    {
        Task<UploadedImageResult> UploadAsync(
            Stream imageStream,
            string fileName,
            string contentType,
            CancellationToken ct);

        Task DeleteAsync(
            string publicId,
             CancellationToken ct);
    }
}