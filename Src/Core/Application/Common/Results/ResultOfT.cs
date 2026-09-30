using Application.Common.Errors;

namespace Application.Common.Results
{
    public class Result<T> : Result
    {
        public T? Value { get; }

        protected Result(bool isSuccess, T? value, Error? error) : base(isSuccess, error)
        {
            Value = value;
        }
     

        public static Result<T> Success(T value)
        {
            return new Result<T>(true, value, null);
        }

        public static new Result<T> Failure(Error error)
        {
            return new Result<T>(false, default, error);
        }
    }
}
