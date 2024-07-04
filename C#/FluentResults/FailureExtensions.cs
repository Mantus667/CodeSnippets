using FluentResults;

namespace FluentResultsFailureExtensions;

public static class FailureExtensions
{
    public static string GetErrorCode(this IResultBase result)
        => result.IsFailed
            ? result.GetString(FailureBuilder.MetadataKey.ErrorCode)
            : string.Empty;

    public static string GetDetails(this IResultBase result)
        => result.IsFailed
            ? result.GetString(FailureBuilder.MetadataKey.Detail)
            : string.Empty;

    public static Exception? GetException(this IResultBase result)
        => result.IsFailed
            ? result.Get<Exception>(FailureBuilder.MetadataKey.ExceptionKey)
            : null;

    public static string GetAccountId(this IResultBase result)
        => result.IsFailed
            ? result.GetString(FailureBuilder.MetadataKey.AccountId)
            : string.Empty;

    private static string GetString(this IResultBase result, string key)
        => result.Get<string>(key) ?? string.Empty;

    private static T? Get<T>(this IResultBase result, string key) where T : class
    {
        var metadata = result.Errors.FirstOrDefault()?.Metadata;
        if (metadata is null)
        {
            return null;
        }

        return metadata.TryGetValue(key, out var value)
            ? value as T
            : null;
    }
}
