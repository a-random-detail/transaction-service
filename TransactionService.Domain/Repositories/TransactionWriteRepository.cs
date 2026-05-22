using Dapper;
using Microsoft.Extensions.Logging;
using Npgsql;
using TransactionService.Domain.Core;
using TransactionService.Domain.Entities;
using TransactionService.Domain.Handlers.Commands;
using TransactionService.Domain.Infrastructure;

namespace TransactionService.Domain.Repositories;

public interface ITransactionWriteRepository
{
    Task<Result<TransactionDto>> CreateAsync(CreateTransactionCommand command);
}

public class TransactionWriteRepository(
    IWriteConnectionFactory connectionFactory,
    ILogger<TransactionWriteRepository> logger
    ): ITransactionWriteRepository
{
    public async Task<Result<TransactionDto>> CreateAsync(CreateTransactionCommand command)
    {
        logger.LogDebug("Inserting Transaction {Description}", command.Description);
        
        using var connection = connectionFactory.Create();
        connection.Open();
        using var tx = connection.BeginTransaction();

        try
        {

            var dto = await connection.QuerySingleAsync<TransactionDto>(
                """
                Insert into transactions (description, transaction_date, amount)
                values (@Description, @TransactionDate, @Amount)
                returning id, description, transaction_date, amount
                """,
                command,
                transaction: tx);
            tx.Commit();
            return Result<TransactionDto>.OK(dto);
        } 
        catch (PostgresException ex) 
        {
            tx.Rollback();
            logger.LogError("Database error inserting transaction {Description} - State: {SqlState} Detail: {Detail}", command.Description, ex.SqlState, ex.Detail);
            return Result<TransactionDto>.Fail(ResultType.ServerError, "Unable to save transaction to database.");
            
        }
        catch (Exception ex)
        {
            tx.Rollback();
            logger.LogError("Database error inserting transaction {Description}", command.Description);
            return Result<TransactionDto>.Fail(ResultType.ServerError, "Unable to save transaction to database.");
        }


    }
}