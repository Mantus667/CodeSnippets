public class Result<TResult, TError>
{
    private readonly TResult _result;
    private readonly TError _error;
    private readonly bool _isError;

    private Result(TResult result, TError error, bool isError)
    {
        _result = result;
        _error = error;
        _isError = isError;
    }

    public Result(TResult result)
        : this(result, default, false)
    {
    }

    public Result(TError error)
        : this(default, error, true)
    {
    }
    
    // ** Do your fancy monad binds and maps here

    public static implicit operator Result<TResult, TError>(DelayedResult<TResult> ok) => new Result<TResult, TError>(ok.Value);

    public static implicit operator Result<TResult, TError>(DelayedResult<TError> error) => new Result<TResult, TError>(error.Value);
}

public class DelayedResult<T>
{
    public T Value { get; }

    public DelayedResult(T value)
    {
        Value = value;
    }
}

public static class Result
{
    public static DelayedResult<TResult> Ok<TResult>(TResult result) => new DelayedResult<TResult>(result);

    public static DelayedResult<TError> Error<TError>(TError error) => new DelayedResult<TError>(error);
}

public static class Program
{
    public static Result<int, string> Parse(string input)
    {
        if (int.TryParse(input, out var value))
            return Result.Ok(value);

        return Result.Error("Invalid value");
    }
}