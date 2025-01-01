namespace Shared.Results
{
    public class Result<TValue> : Result
    {
        private Result(TValue? value, bool isSuccess, Error error)
            :base(isSuccess, error)
        {
            Value = value;
        }

        private Result(TValue? value, bool isSuccess)
            : base(isSuccess)
        {
            Value = value;
        }

        public TValue? Value { get; }

        public static implicit operator Result<TValue>(TValue data) => new(data, true);

        public static implicit operator Result<TValue>(Error error) => new(default, false, error);
    }
}
