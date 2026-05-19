namespace TransactionService.Domain.Core;

public class Result<T>
{
    private Result() { }

    private T? Value { get; init; }
    private List<string> Errors { get; init; } = [];
    
    public bool Success => Errors.Count == 0;
    public T? GetValue() => Success ? Value : default;
    public IReadOnlyList<string> GetErrors() => Errors;

    public static Result<T> OK(T data) => new() { Value = data };
    public static Result<T> Fail(params string[] errors) => new() { Errors = [..errors] };
}
