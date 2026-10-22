
namespace Application.ResponseDTO
{
    public sealed record DashboardResponse(
        int TotalActiveUsers,
        int TotalUnActiveUser,
        int TotalTickets,
        int TotalUnAssignedTickets);
}
