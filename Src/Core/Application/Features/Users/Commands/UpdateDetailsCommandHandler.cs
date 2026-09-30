using Application.Common.Errors;
using Application.Common.Results;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Interfaces.Uof;
using MediatR;

namespace Application.Features.Users.Commands
{
    public sealed class UpdateDetailsCommandHandler : IRequestHandler<UpdateDetailsCommand, Result>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserWriteRepository _userWriteRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDetailsCommandHandler(
            ICurrentUserService currentUserService,
            IUserWriteRepository userWriteRepository,
            IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _userWriteRepository = userWriteRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateDetailsCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;

            var user = await _userWriteRepository.GetUserByIdAsync(currentUserId, cancellationToken);
            if (user is null)
                return Result.Failure(UserErrors.UserNotFound);

            user.UpdateFirstName(request.FirstName);
            user.UpdateLastName(request.LastName);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
