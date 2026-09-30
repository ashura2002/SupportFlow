using Domain.Enums;

namespace Application.Common.Errors
{
    public sealed record Error(string Code, string Message, ErrorType Type);
}
