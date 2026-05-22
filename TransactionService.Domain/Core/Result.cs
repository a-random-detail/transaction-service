namespace TransactionService.Domain.Core;

public enum ResultType { Ok, NotFound, ValidationError, NoRecordsFound, ServerError }
public class Result<T>
{
    private Result() { }

    private T? Value { get; init; }
    private IReadOnlyList<string> Errors { get; init; } = [];
    
    public ResultType Type { get; init; }
    public bool Success => Type == ResultType.Ok;
    public T? GetValue() => Success ? Value : default;
    public IReadOnlyList<string> GetErrors() => Errors;

    public static Result<T> OK(T? data) => new() { Type = ResultType.Ok, Value = data };

    public static Result<T> Fail(ResultType type, params string[] errors)
    {
        if (type == ResultType.Ok) throw new ArgumentException("ResultType.Ok is not a valid failure type.", nameof(type)); 
        return new() { Type = type, Errors = [..errors] };  
    } 
}
