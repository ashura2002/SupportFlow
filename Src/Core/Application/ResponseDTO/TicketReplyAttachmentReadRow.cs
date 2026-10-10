namespace Application.ResponseDTO;

public sealed record TicketReplyAttachmentReadRow(
    Guid ReplyId,
    string PublicImageUrl,
    string PublicImageId
);
