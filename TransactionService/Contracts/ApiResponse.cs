namespace TransactionService.Contracts;

public class ApiResponse<T>
{
    public T? Data { get; init; }
    public List<string> Errors { get; init; }
    public bool Success => Errors.Count == 0;
}