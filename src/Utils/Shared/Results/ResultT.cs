namespace Shared.Results
{
    public class Result<TValue>
    {
        private Result(TValue? value, bool isSuccess, Error error)
        {
            Value = value;
            IsSuccess = isSuccess;
            Error = error;
        }

        private Result(TValue? value, bool isSuccess)
        {
            Value = value;
            IsSuccess = isSuccess;
        }

        public bool IsSuccess { get; }

        public bool IsFailure => !IsSuccess;

        public Error? Error { get; }

        public TValue? Value { get; }

        public static Result<TValue> Success(TValue value) => new Result<TValue>(value, true);

        public static Result<TValue> Failure(Error error) => new(default, false, error);
    }
}
