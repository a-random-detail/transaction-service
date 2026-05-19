namespace TransactionService.Domain.Core;

public class Result<T>
{
    public T? Value { get; init; }
    public List<string> Errors { get; init; } = [];
    public bool Success => Errors.Count == 0;

    public static Result<T> OK(T data) => new() { Value = data };
    public static Result<T> Fail(params string[] errors) => new() { Errors = [..errors] };
}
