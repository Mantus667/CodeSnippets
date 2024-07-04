using FluentResults;

namespace FluentResultsExtensions;

public static class ResultExtensions
{
    /// <summary>
    /// Performs a side effect on a successful input result forwards the input result
    /// when the side effect succeeds, else a failure result.
    /// </summary>
    /// <typeparam name="T">Generic type of the input result.</typeparam>
    /// <param name="input">The input result.</param>
    /// <param name="sideEffect">The side effect operation.</param>
    /// <returns>A result with same generic parameter.</returns>
    public static Result<T> Tap<T>(
        this Result<T> input,
        Func<T, Result> sideEffect)
    {
        if (input.IsFailed)
        {
            return input;
        }

        var sideEffectResult = sideEffect(input.Value);

        return sideEffectResult.IsSuccess
            ? input
            : sideEffectResult;
    }

    /// <summary>
    /// Performs an asynchronous side effect on a successful input result and forwards the input result
    /// when the side effect succeeds, else a failure result.
    /// </summary>
    /// <typeparam name="T">Generic type of the input result.</typeparam>
    /// <param name="inputTask">The input task yielding a result.</param>
    /// <param name="sideEffect">The asynchronous side effect operation.</param>
    /// <returns>A result with same generic parameter.</returns>
    public static async Task<Result<T>> Tap<T>(
        this Task<Result<T>> inputTask,
        Func<T, Task<Result>> sideEffect)
    {
        var result = await inputTask;
        if (result.IsFailed)
        {
            return result;
        }

        var sideEffectResult = await sideEffect(result.Value);

        return sideEffectResult.IsSuccess
            ? result
            : sideEffectResult;
    }

    /// <summary>
    /// Performs a side effect on a successful input result when it satisfies a condition.
    /// It forwards the input result when the side effect succeeds or the condition is not met,
    /// else a failure result.
    /// </summary>
    /// <typeparam name="T">Generic type of the result.</typeparam>
    /// <param name="input">The input result.</param>
    /// <param name="condition">The condition which determines whether the side effect shall be executed.</param>
    /// <param name="sideEffect">The side effect operation.</param>
    /// <returns>A result with same generic parameter.</returns>
    public static Result<T> TapIf<T>(
        this Result<T> input,
        Func<T, bool> condition,
        Func<T, Result> sideEffect)
        => input.IsFailed || !condition(input.Value)
            ? input
            : input.Tap(sideEffect);

    /// <summary>
    /// Performs an asynchronous side effect on a successful input result when it satisfies a condition.
    /// It forwards the input result when the side effect succeeds or the condition is not met,
    /// else a failure result.
    /// </summary>
    /// <typeparam name="T">Generic type of the result.</typeparam>
    /// <param name="inputTask">The input task yielding a result.</param>
    /// <param name="condition">The condition which determines whether the side effect shall be executed.</param>
    /// <param name="sideEffect">The side effect operation.</param>
    /// <returns>A result with same generic parameter.</returns>
    public static async Task<Result<T>> TapIf<T>(
        this Task<Result<T>> inputTask,
        Func<T, bool> condition,
        Func<T, Task<Result>> sideEffect)
    {
        var result = await inputTask;
        if (result.IsFailed || !condition(result.Value))
        {
            return result;
        }

        var sideEffectResult = await sideEffect(result.Value);

        return sideEffectResult.IsSuccess
            ? result
            : sideEffectResult;
    }

    /// <summary>
    /// Produces a generic result of given value from a non-generic one.
    /// </summary>
    /// <typeparam name="T">Generic type of the result.</typeparam>
    /// <param name="input">The non-generic input result.</param>
    /// <param name="value">The value of the output result.</param>
    /// <returns>A generic result.</returns>
    public static Result<T> Produce<T>(this Result input, T value)
        => input.IsSuccess
            ? Result.Ok(value)
            : input;

    /// <summary>
    /// Produces a generic result of given value from a non-generic one.
    /// </summary>
    /// <typeparam name="T">Generic type of the result.</typeparam>
    /// <param name="inputTask">The input task yielding a non-generic result.</param>
    /// <param name="value">The value of the output result.</param>
    /// <returns>A generic result.</returns>
    public static async Task<Result<T>> Produce<T>(this Task<Result> inputTask, T value)
        => (await inputTask).Produce(value);


    /// <summary>
    /// Produces a generic result of given value from a non-generic one.
    /// </summary>
    /// <typeparam name="T">Generic type of the result.</typeparam>
    /// <param name="inputTask">The input ValueTask yielding a non-generic result.</param>
    /// <param name="value">The value of the output result.</param>
    /// <returns>A generic result.</returns>
    public static async ValueTask<Result<T>> Produce<T>(this ValueTask<Result> inputTask, T value)
        => (await inputTask).Produce(value);

    /// <summary>
    /// Appends the value of a successful input result by a value produced by a given function,
    /// hence producing a tuple.
    /// Fails, when the input is a failed result or when the function returns a failure.
    /// </summary>
    /// <typeparam name="TExisting">Generic type of the input result.</typeparam>
    /// <typeparam name="TNew">Generic type of the result that is produced by the function.</typeparam>
    /// <param name="input">The input result.</param>
    /// <param name="function">The function which is used to create the second entry in the tuple.</param>
    /// <returns>A result holding a tuple of values.</returns>
    public static Result<(TExisting PassedThrough, TNew Produced)> Append<TExisting, TNew>(
        this Result<TExisting> input,
        Func<TExisting, Result<TNew>> function)
    {
        if (input.IsFailed)
        {
            return input.ToResult<(TExisting, TNew)>();
        }

        var newResult = function(input.Value);
        return newResult.IsSuccess
            ? Result.Ok((input.Value, newResult.Value))
            : newResult.ToResult<(TExisting, TNew)>();
    }

    /// <summary>
    /// Appends the value of a successful input result by a value produced by a given asynchronous function,
    /// hence producing a tuple.
    /// Fails, when the input is a failed result or when the function returns a failure.
    /// </summary>
    /// <typeparam name="TExisting">Generic type of the input result.</typeparam>
    /// <typeparam name="TNew">Generic type of the result that is produced by the function.</typeparam>
    /// <param name="input">The input result.</param>
    /// <param name="function">The asynchronous function which is used to create the second entry in the tuple.</param>
    /// <returns>A result holding a tuple of values.</returns>
    public static async Task<Result<(TExisting PassedThrough, TNew Produced)>> Append<TExisting, TNew>(
        this Result<TExisting> input,
        Func<TExisting, Task<Result<TNew>>> function)
    {
        if (input.IsFailed)
        {
            return input.ToResult<(TExisting, TNew)>();
        }

        var newResult = await function(input.Value);
        return newResult.IsSuccess
            ? Result.Ok((input.Value, newResult.Value))
            : newResult.ToResult<(TExisting, TNew)>();
    }

    /// <summary>
    /// Appends the value of a successful input result by a value produced by a given asynchronous function,
    /// hence producing a tuple.
    /// Fails, when the input is a failed result or when the function returns a failure.
    /// </summary>
    /// <typeparam name="TExisting">Generic type of the input result.</typeparam>
    /// <typeparam name="TNew">Generic type of the result that is produced by the function.</typeparam>
    /// <param name="inputTask">The task yielding an input result.</param>
    /// <param name="function">The asynchronous function which is used to create the second entry in the tuple.</param>
    /// <returns>A result holding a tuple of values.</returns>
    public static async Task<Result<(TExisting PassedThrough, TNew Produced)>> Append<TExisting, TNew>(
        this Task<Result<TExisting>> inputTask,
        Func<TExisting, Task<Result<TNew>>> function)
    {
        var result = await inputTask;
        if (result.IsFailed)
        {
            return result.ToResult<(TExisting, TNew)>();
        }

        var newResult = await function(result.Value);
        return newResult.IsSuccess
            ? Result.Ok((result.Value, newResult.Value))
            : newResult.ToResult<(TExisting, TNew)>();
    }

    /// <summary>
    /// Appends the value of a successful input result by a value, hence producing a tuple,
    /// which is either the default value for the generic type or the output of a given function,
    /// depending on whether the given condition is met.
    /// Fails, when the input is a failed result or when the function returns a failure.
    /// </summary>
    /// <typeparam name="TExisting">Generic type of the input result.</typeparam>
    /// <typeparam name="TNew">Generic type of the result that is produced by the function.</typeparam>
    /// <param name="input">The input result.</param>
    /// <param name="condition">The condition which determines whether the function shall be executed.</param>
    /// <param name="function">The function which is used to create the second entry in the tuple.</param>
    /// <returns>A result holding a tuple of values.</returns>
    public static Result<(TExisting PassedThrough, TNew? Produced)> AppendIf<TExisting, TNew>(
        this Result<TExisting> input,
        Func<TExisting, bool> condition,
        Func<TExisting, Result<TNew>> function)
    {
        if (input.IsFailed || !condition(input.Value))
        {
            return input.ToResult<(TExisting, TNew?)>(existing => (existing, default));
        }

        var newResult = function(input.Value);
        return newResult.IsSuccess
            ? Result.Ok((input.Value, (TNew?)newResult.Value))
            : newResult.ToResult<(TExisting, TNew?)>(_ => (input.Value, default));
    }

    /// <summary>
    /// Appends the value of a successful input result by a value, hence producing a tuple,
    /// which is either the default value for the generic type or the output of a given asynchronous function,
    /// depending on whether the given condition is met.
    /// Fails, when the input is a failed result or when the function returns a failure.
    /// </summary>
    /// <typeparam name="TExisting">Generic type of the input result.</typeparam>
    /// <typeparam name="TNew">Generic type of the result that is produced by the function.</typeparam>
    /// <param name="input">The input result.</param>
    /// <param name="condition">The condition which determines whether the function shall be executed.</param>
    /// <param name="function">The function which is used to create the second entry in the tuple.</param>
    /// <returns>A result holding a tuple of values.</returns>
    public static async Task<Result<(TExisting PassedThrough, TNew? Produced)>> AppendIf<TExisting, TNew>(
        this Result<TExisting> input,
        Func<TExisting, bool> condition,
        Func<TExisting, Task<Result<TNew>>> function)
    {
        if (input.IsFailed || !condition(input.Value))
        {
            return input.ToResult<(TExisting, TNew?)>(existing => (existing, default));
        }

        var newResult = await function(input.Value);
        return newResult.IsSuccess
            ? Result.Ok((input.Value, (TNew?)newResult.Value))
            : newResult.ToResult<(TExisting, TNew?)>(_ => (input.Value, default));
    }

    /// <summary>
    /// Appends the value of a successful input result by a value, hence producing a tuple,
    /// which is either the default value for the generic type or the output of a given asynchronous function,
    /// depending on whether the given condition is met.
    /// Fails, when the input is a failed result or when the function returns a failure.
    /// </summary>
    /// <typeparam name="TExisting">Generic type of the input result.</typeparam>
    /// <typeparam name="TNew">Generic type of the result that is produced by the function.</typeparam>
    /// <param name="input">The input task yielding a result.</param>
    /// <param name="condition">The condition which determines whether the function shall be executed.</param>
    /// <param name="function">The function which is used to create the second entry in the tuple.</param>
    /// <returns>A result holding a tuple of values.</returns>
    public static async Task<Result<(TExisting PassedThrough, TNew? Produced)>> AppendIf<TExisting, TNew>(
        this Task<Result<TExisting>> input,
        Func<TExisting, bool> condition,
        Func<TExisting, Task<Result<TNew>>> function)
    {
        var result = await input;
        if (result.IsFailed || !condition(result.Value))
        {
            return result.ToResult<(TExisting, TNew?)>(existing => (existing, default));
        }

        var newResult = await function(result.Value);
        return newResult.IsSuccess
            ? Result.Ok((result.Value, (TNew?)newResult.Value))
            : newResult.ToResult<(TExisting, TNew?)>(_ => (result.Value, default));
    }
}
