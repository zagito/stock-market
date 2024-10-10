namespace Shared.Results
{
    public class Result
    {
        protected Result(bool isSuccess) 
        {
            IsSuccess = isSuccess;
        }

        protected Result(bool isSuccess, Error error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public bool IsSuccess { get; }

        public bool IsFailure => !IsSuccess;

        public Error? Error { get; }

        public static Result Success() => new(true);

        public static Result Failure(Error error) => new(false, error);
    }
}
