namespace Application.ResponseDTO
{
  public sealed record TicketReplyResponse(
    Guid Id,
    Guid AuthorId,
    string AuthorFullName,
    string Message,
    IReadOnlyCollection<UploadedImageResult>? Attachments,
    DateTime CreatedAt);
}
