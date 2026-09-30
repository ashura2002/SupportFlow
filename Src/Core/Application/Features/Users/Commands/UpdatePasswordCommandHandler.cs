using Application.Common.Errors;
using Application.Common.Results;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Interfaces.Uof;
using MediatR;

namespace Application.Features.Users.Commands
{
    public sealed class UpdatePasswordCommandHandler : IRequestHandler<UpdatePasswordCommand, Result>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserWriteRepository _userWriteRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordService _passwordService;

        public UpdatePasswordCommandHandler(
            ICurrentUserService currentUserService,
            IUserWriteRepository userWriteRepository,
            IUnitOfWork unitOfWork,
            IPasswordService passwordService)
        {
            _currentUserService = currentUserService;
            _userWriteRepository = userWriteRepository;
            _unitOfWork = unitOfWork;
            _passwordService = passwordService;
        }


        public async Task<Result> Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;

            var user = await _userWriteRepository.GetUserByIdAsync(currentUserId, cancellationToken);
            if (user is null)
                return Result.Failure(UserErrors.UserNotFound);

            var passwordHash = _passwordService.HashPassword(request.Password);
            user.UpdatePassword(passwordHash);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();

        }
    }
}
