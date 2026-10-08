
namespace Application.ResponseDTO
{
    public sealed record NotificationResponse(
        Guid Id,
        string Content,
        bool IsRead);
}
