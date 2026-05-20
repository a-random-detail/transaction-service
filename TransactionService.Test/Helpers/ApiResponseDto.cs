namespace TransactionService.Test.Helpers;

// This record mirrors the structure of the ApiResponse contract within the service project 
// So that we do not have to make the constructor public on ApiResponse which could lead to conflicting state within a class instance
public record ApiResponseDto<T>(T? Data, IReadOnlyList<string> Errors, bool Success);