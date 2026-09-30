using FluentValidation;
using MediatR;

namespace Application.Behaviors
{
    public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }


        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any())
                return await next(cancellationToken);

            // create vaidation context for the current rquest
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            // flatten all the validation errors from all validators to a single list
            var failureResults = validationResults.SelectMany(error => error.Errors)
                .Where(error => error is not null)
                .ToList();

            // if one or more exist stop the pipeline by throwing validation exception
            if (failureResults.Count != 0)
                throw new ValidationException(failureResults);

            return await next(cancellationToken);
        }
    }
}
