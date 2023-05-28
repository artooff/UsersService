namespace UsersService.Application.Common
{
    public record Result<TResult>
    {
        public bool IsSuccess { get; init; }
        public TResult Value { get; init; }
        public Exception Exception { get; init; }
    }
}
