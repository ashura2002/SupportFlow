using Application.Common.Errors;
using Application.Common.Results;
using Application.Features.Users.Commands;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Interfaces.Uof;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using MediatR;

namespace Application.Features.Repositories.Commands
{
    public sealed class CreateRequesterCommandHandler : IRequestHandler<CreateRequesterCommand, Result<Guid>>
    {
        private readonly IUserWriteRepository _userWriteRepository;
        private readonly IUserReadRepository _userReadRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordService _passwordService;

        public CreateRequesterCommandHandler(
            IUserWriteRepository userWriteRepository,
            IUserReadRepository userReadRepository,
            IUnitOfWork unitOfWork,
            IPasswordService passwordService)
        {
            _userWriteRepository = userWriteRepository;
            _userReadRepository = userReadRepository;
            _unitOfWork = unitOfWork;
            _passwordService = passwordService;
        }

        public async Task<Result<Guid>> Handle(CreateRequesterCommand request, CancellationToken cancellationToken)
        {
            var email = Email.Create(request.Email);

            if (await _userReadRepository.IsEmailExist(email.Value, cancellationToken))
                return Result<Guid>.Failure(UserErrors.EmailExist);

            var passwordHash = _passwordService.HashPassword(request.Password);
            var user = User.Create(
                request.FirstName,
                request.LastName,
                passwordHash, 
                email, 
                Roles.Requester);

            _userWriteRepository.Add(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(user.Id);
        }
    }
}
