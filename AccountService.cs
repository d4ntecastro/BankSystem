using System;
using System.Collections.Generic;
using System.Linq;

namespace BankSystem
{
    /// <summary>
    /// Implementation of the IAccountService interface.
    /// Manages a collection of bank accounts with support for creating accounts,
    /// deposits, withdrawals, and persistent data storage.
    /// 
    /// This class encapsulates the business logic for account operations and
    /// maintains the collection of Account objects in memory.
    /// </summary>
    public class AccountService : IAccountService
    {
        /// <summary>
        /// In-memory collection of accounts.
        /// </summary>
        private List<Account> _accounts;

        /// <summary>
        /// Data persistence handler for file-based storage.
        /// </summary>
        private readonly DataPersistence _dataPersistence;

        /// <summary>
        /// Initializes a new instance of the AccountService class.
        /// Automatically loads existing account data from persistent storage.
        /// </summary>
        public AccountService()
        {
            _dataPersistence = new DataPersistence();
            _accounts = new List<Account>();

            // Load existing data automatically
            LoadData();
        }

        /// <summary>
        /// Creates a new bank account with the specified account holder name.
        /// Initializes the account with zero balance and creates an account creation transaction record.
        /// </summary>
        /// <param name="accountHolder">The name of the account holder</param>
        /// <returns>The newly created Account object</returns>
        /// <exception cref="ArgumentException">Thrown when accountHolder is null or empty</exception>
        public Account CreateAccount(string accountHolder)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(accountHolder))
            {
                throw new ArgumentException("Account holder name cannot be null or empty.", nameof(accountHolder));
            }

            // Create new account
            Account newAccount = new Account(accountHolder);

            // Create account creation transaction
            Transaction creationTransaction = new Transaction(
                Transaction.TransactionType.AccountCreation,
                0m,
                0m,
                "Account created"
            );
            newAccount.Transactions.Add(creationTransaction);

            // Add to collection
            _accounts.Add(newAccount);

            // Persist changes
            SaveData();

            Console.WriteLine($"✓ Account created successfully. Account Number: {newAccount.AccountNumber}");
            return newAccount;
        }

        /// <summary>
        /// Deposits funds into an existing account.
        /// Creates a transaction record for the deposit.
        /// </summary>
        /// <param name="accountNumber">The account number to deposit into</param>
        /// <param name="amount">The amount to deposit (must be positive)</param>
        /// <returns>The updated Account object</returns>
        /// <exception cref="ArgumentException">Thrown when amount is negative or zero</exception>
        /// <exception cref="InvalidOperationException">Thrown when account is not found or is inactive</exception>
        public Account Deposit(string accountNumber, decimal amount)
        {
            // Validate amount
            if (amount <= 0)
            {
                throw new ArgumentException("Deposit amount must be greater than zero.", nameof(amount));
            }

            // Find account
            Account account = GetAccount(accountNumber);
            if (account == null)
            {
                throw new InvalidOperationException($"Account {accountNumber} not found.");
            }

            // Check if account is active
            if (!account.IsActive)
            {
                throw new InvalidOperationException($"Account {accountNumber} is inactive.");
            }

            // Update balance
            account.Balance += amount;
            account.LastModifiedDate = DateTime.Now;

            // Create transaction record
            Transaction transaction = new Transaction(
                Transaction.TransactionType.Deposit,
                amount,
                account.Balance,
                $"Deposit of ${amount:F2}"
            );
            account.Transactions.Add(transaction);

            // Persist changes
            SaveData();

            Console.WriteLine($"✓ Deposit successful. New balance: ${account.Balance:F2}");
            return account;
        }

        /// <summary>
        /// Withdraws funds from an existing account.
        /// Validates that sufficient funds are available before processing withdrawal.
        /// Creates a transaction record for the withdrawal.
        /// </summary>
        /// <param name="accountNumber">The account number to withdraw from</param>
        /// <param name="amount">The amount to withdraw (must be positive)</param>
        /// <returns>The updated Account object</returns>
        /// <exception cref="ArgumentException">Thrown when amount is negative, zero, or exceeds balance</exception>
        /// <exception cref="InvalidOperationException">Thrown when account is not found or is inactive</exception>
        public Account Withdraw(string accountNumber, decimal amount)
        {
            // Validate amount
            if (amount <= 0)
            {
                throw new ArgumentException("Withdrawal amount must be greater than zero.", nameof(amount));
            }

            // Find account
            Account account = GetAccount(accountNumber);
            if (account == null)
            {
                throw new InvalidOperationException($"Account {accountNumber} not found.");
            }

            // Check if account is active
            if (!account.IsActive)
            {
                throw new InvalidOperationException($"Account {accountNumber} is inactive.");
            }

            // Check for sufficient funds
            if (account.Balance < amount)
            {
                throw new ArgumentException(
                    $"Insufficient funds. Current balance: ${account.Balance:F2}, Requested withdrawal: ${amount:F2}",
                    nameof(amount)
                );
            }

            // Update balance
            account.Balance -= amount;
            account.LastModifiedDate = DateTime.Now;

            // Create transaction record
            Transaction transaction = new Transaction(
                Transaction.TransactionType.Withdrawal,
                amount,
                account.Balance,
                $"Withdrawal of ${amount:F2}"
            );
            account.Transactions.Add(transaction);

            // Persist changes
            SaveData();

            Console.WriteLine($"✓ Withdrawal successful. New balance: ${account.Balance:F2}");
            return account;
        }

        /// <summary>
        /// Retrieves an account by its account number.
        /// </summary>
        /// <param name="accountNumber">The account number to retrieve</param>
        /// <returns>The Account object if found; null otherwise</returns>
        public Account GetAccount(string accountNumber)
        {
            return _accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
        }

        /// <summary>
        /// Retrieves all accounts in the system.
        /// Returns a copy of the list to prevent external modification.
        /// </summary>
        /// <returns>A list of all Account objects</returns>
        public List<Account> GetAllAccounts()
        {
            return new List<Account>(_accounts);
        }

        /// <summary>
        /// Gets the current balance of an account.
        /// </summary>
        /// <param name="accountNumber">The account number</param>
        /// <returns>The current balance</returns>
        /// <exception cref="InvalidOperationException">Thrown when account is not found</exception>
        public decimal GetBalance(string accountNumber)
        {
            Account account = GetAccount(accountNumber);
            if (account == null)
            {
                throw new InvalidOperationException($"Account {accountNumber} not found.");
            }

            return account.Balance;
        }

        /// <summary>
        /// Gets the transaction history for a specific account.
        /// Returns a copy of the transaction list.
        /// </summary>
        /// <param name="accountNumber">The account number</param>
        /// <returns>A list of Transaction objects for the account</returns>
        /// <exception cref="InvalidOperationException">Thrown when account is not found</exception>
        public List<Transaction> GetTransactionHistory(string accountNumber)
        {
            Account account = GetAccount(accountNumber);
            if (account == null)
            {
                throw new InvalidOperationException($"Account {accountNumber} not found.");
            }

            return new List<Transaction>(account.Transactions);
        }

        /// <summary>
        /// Deletes an account from the system by marking it as inactive.
        /// The account data is preserved but operations on the account are prevented.
        /// </summary>
        /// <param name="accountNumber">The account number to delete</param>
        /// <returns>True if account was successfully deleted; false otherwise</returns>
        public bool DeleteAccount(string accountNumber)
        {
            Account account = GetAccount(accountNumber);
            if (account == null)
            {
                Console.WriteLine($"Account {accountNumber} not found.");
                return false;
            }

            account.IsActive = false;
            account.LastModifiedDate = DateTime.Now;
            SaveData();

            Console.WriteLine($"✓ Account {accountNumber} has been deleted (marked as inactive).");
            return true;
        }

        /// <summary>
        /// Persists all current account data to permanent storage (JSON file).
        /// </summary>
        /// <returns>True if save was successful; false otherwise</returns>
        public bool SaveData()
        {
            return _dataPersistence.SaveAccounts(_accounts);
        }

        /// <summary>
        /// Loads account data from permanent storage (JSON file).
        /// Replaces the current in-memory collection with loaded data.
        /// </summary>
        /// <returns>True if load was successful; false otherwise</returns>
        public bool LoadData()
        {
            _accounts = _dataPersistence.LoadAccounts();
            return true;
        }

        /// <summary>
        /// Gets the number of accounts in the system (including inactive accounts).
        /// </summary>
        /// <returns>The account count</returns>
        public int GetAccountCount()
        {
            return _accounts.Count;
        }

        /// <summary>
        /// Gets the number of active accounts in the system.
        /// </summary>
        /// <returns>The active account count</returns>
        public int GetActiveAccountCount()
        {
            return _accounts.Count(a => a.IsActive);
        }

        /// <summary>
        /// Gets the total balance across all active accounts.
        /// </summary>
        /// <returns>The total balance</returns>
        public decimal GetTotalBalance()
        {
            return _accounts.Where(a => a.IsActive).Sum(a => a.Balance);
        }

        /// <summary>
        /// Clears all data from memory and storage.
        /// WARNING: This operation cannot be undone.
        /// </summary>
        public void ClearAllData()
        {
            _accounts.Clear();
            _dataPersistence.DeleteDataFile();
            Console.WriteLine("All data cleared.");
        }
    }
}
