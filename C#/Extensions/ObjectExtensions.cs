/// <summary>
/// Provides a set of extensions that extend objects.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Execute and returns the result of the <paramref name="func"/> expression only if <paramref name="self"/> is not null. 
    /// If <paramref name="self"/> is null it will return the default of <typeparamref name="TResult"/>.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="self"></param>
    /// <param name="func"></param>
    /// <returns>The result of the func if self is not null, otherwise the default value of TResult.</returns>
    public static TResult IsNotNull<TItem, TResult>(this TItem self, Func<TItem, TResult> func) where TItem : class
    {
        Contract.Requires(func != null);

        return self.IsNotNull(func, default(TResult));
    }

    /// <summary>
    /// Execute and returns the result of the <paramref name="func"/> expression only if <paramref name="self"/> is not null. 
    /// If <paramref name="self"/> is null it will return <paramref name="defaultValue"/>.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="self"></param>
    /// <param name="func"></param>
    /// <param name="defaultValue"></param>
    /// <returns>The result of the func if self is not null, otherwise the default value.</returns>
    public static TResult IsNotNull<TItem, TResult>(this TItem self, Func<TItem, TResult> func, TResult defaultValue)
        where TItem : class
    {
        Contract.Requires(func != null);

        if (self == null)
        {
            return defaultValue;
        }

        return func(self);
    }
}