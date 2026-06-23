using System;
using System.Collections.Generic;

namespace BankSystem
{
    /// <summary>
    /// Abstract Data Type (ADT) interface for bank account operations.
    /// Defines the contract for all account management operations including creation, 
    /// deposits, withdrawals, and retrieval of account information.
    /// 
    /// This interface encapsulates the essential operations on a bank account collection,
    /// providing a clear separation between interface (what operations are available) and
    /// implementation (how those operations are performed).
    /// </summary>
    public interface IAccountService
    {
        /// <summary>
        /// Creates a new bank account with the specified account holder name.
        /// </summary>
        /// <param name="accountHolder">The name of the account holder</param>
        /// <returns>The newly created Account object</returns>
        /// <exception cref="ArgumentException">Thrown when accountHolder is null or empty</exception>
        Account CreateAccount(string accountHolder);

        /// <summary>
        /// Deposits funds into an existing account.
        /// </summary>
        /// <param name="accountNumber">The account number to deposit into</param>
        /// <param name="amount">The amount to deposit (must be positive)</param>
        /// <returns>The updated Account object</returns>
        /// <exception cref="ArgumentException">Thrown when amount is negative or zero</exception>
        /// <exception cref="InvalidOperationException">Thrown when account is not found or is inactive</exception>
        Account Deposit(string accountNumber, decimal amount);

        /// <summary>
        /// Withdraws funds from an existing account.
        /// </summary>
        /// <param name="accountNumber">The account number to withdraw from</param>
        /// <param name="amount">The amount to withdraw (must be positive)</param>
        /// <returns>The updated Account object</returns>
        /// <exception cref="ArgumentException">Thrown when amount is negative, zero, or exceeds balance</exception>
        /// <exception cref="InvalidOperationException">Thrown when account is not found or is inactive</exception>
        Account Withdraw(string accountNumber, decimal amount);

        /// <summary>
        /// Retrieves an account by its account number.
        /// </summary>
        /// <param name="accountNumber">The account number to retrieve</param>
        /// <returns>The Account object if found; null otherwise</returns>
        Account GetAccount(string accountNumber);

        /// <summary>
        /// Retrieves all accounts in the system.
        /// </summary>
        /// <returns>A list of all Account objects</returns>
        List<Account> GetAllAccounts();

        /// <summary>
        /// Gets the current balance of an account.
        /// </summary>
        /// <param name="accountNumber">The account number</param>
        /// <returns>The current balance</returns>
        /// <exception cref="InvalidOperationException">Thrown when account is not found</exception>
        decimal GetBalance(string accountNumber);

        /// <summary>
        /// Gets the transaction history for a specific account.
        /// </summary>
        /// <param name="accountNumber">The account number</param>
        /// <returns>A list of Transaction objects for the account</returns>
        /// <exception cref="InvalidOperationException">Thrown when account is not found</exception>
        List<Transaction> GetTransactionHistory(string accountNumber);

        /// <summary>
        /// Deletes an account from the system (marks as inactive).
        /// </summary>
        /// <param name="accountNumber">The account number to delete</param>
        /// <returns>True if account was successfully deleted; false otherwise</returns>
        bool DeleteAccount(string accountNumber);

        /// <summary>
        /// Persists all current account data to permanent storage.
        /// </summary>
        /// <returns>True if save was successful; false otherwise</returns>
        bool SaveData();

        /// <summary>
        /// Loads account data from permanent storage.
        /// </summary>
        /// <returns>True if load was successful; false otherwise</returns>
        bool LoadData();
    }
}
