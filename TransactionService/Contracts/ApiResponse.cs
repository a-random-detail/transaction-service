namespace TransactionService.Contracts;

public class ApiResponse<T>
{
    private ApiResponse() { }

    public T? Data { get; init; }
    public IReadOnlyList<string> Errors { get; init; } = [];
    public bool Success => Errors.Count == 0;

    public static ApiResponse<T> OK(T? data) => new() { Data = data };
    public static ApiResponse<T> Fail(params string[] errors) => new() { Errors = [..errors] };

}