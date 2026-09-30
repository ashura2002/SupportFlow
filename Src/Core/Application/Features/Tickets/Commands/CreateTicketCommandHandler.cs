using Application.Common.Errors;
using Application.Common.Results;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Interfaces.Uof;
using Domain.Entities;
using MediatR;

namespace Application.Features.Tickets.Commands
{
    public sealed class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, Result<Guid>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITicketWriteRepository _ticketWriteRepository;
        private readonly ITicketNumberGeneratorService _ticketNumberGeneratorService;
        private readonly ICategoryReadRepository _categoryReadRepository;

        public CreateTicketCommandHandler(
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork,
            ITicketWriteRepository ticketWriteRepository,
            ITicketNumberGeneratorService ticketNumberGeneratorService,
            ICategoryReadRepository categoryReadRepository)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _ticketWriteRepository = ticketWriteRepository;
            _ticketNumberGeneratorService = ticketNumberGeneratorService;
            _categoryReadRepository = categoryReadRepository;
        }

        public async Task<Result<Guid>> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
        {
            var category = await _categoryReadRepository.GetCategoryByIdAsync(request.CategoryId, cancellationToken);
            if (category is null)
                return Result<Guid>.Failure(CategoryErrors.CategoryNotFound);

            var currentUserId = _currentUserService.UserId;
            var ticketNumber = await _ticketNumberGeneratorService.GenerateTicketNumber(cancellationToken);


            var ticket = Ticket.Create(
                ticketNumber,
                request.Title, 
                request.Description, 
                request.Priority, 
                request.CategoryId, 
                currentUserId);

            _ticketWriteRepository.Add(ticket);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(ticket.Id);

        }
    }
}
