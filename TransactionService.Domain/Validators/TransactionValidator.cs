using System.Globalization;
using TransactionService.Domain.Entities;

namespace TransactionService.Domain.Validators;

public interface ITransactionValidator
{
    bool IsValid(CreateTransactionRequest request, out List<string> errors);
}

public class TransactionValidator: ITransactionValidator
{
    public bool IsValid(CreateTransactionRequest request, out List<string> errors)
    {
        List<string> compiledErrors = new();

        if (string.IsNullOrWhiteSpace(request.Description) || request.Description.Length > 50)
        {
            compiledErrors.Add("Description must be 50 characters or less in length.");
        }
        
        if (!DateTime.TryParseExact(request.TransactionDate, "yyyy-MM-dd",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
        {
            compiledErrors.Add("Transaction date must be in yyyy-MM-dd format.");
        }
        
        if (request.Amount <= 0 || request.Amount != Math.Round(request.Amount, 2))
        {
            compiledErrors.Add("Amount must be a positive number rounded to two decimal places (ex. 99.99).");
        }

        errors = compiledErrors;

        return compiledErrors.Count == 0;
    }
}
