namespace Application.Common.Models;

public sealed record UploadImage(
    Stream Stream,
    string FileName,
    string ContentType,
    long FileSize
);
