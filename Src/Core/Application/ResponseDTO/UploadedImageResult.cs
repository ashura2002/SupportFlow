namespace Application.ResponseDTO;

public sealed record UploadedImageResult(
    string PublicImageUrl,
    string PublicImageId);
