using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace BankSystem
{
    /// <summary>
    /// Represents a bank account with account number, holder name, balance, and transaction history.
    /// This class serves as the data model for the bank system.
    /// </summary>
    public class Account
    {
        /// <summary>
        /// Gets or sets the unique account number. Auto-generated if not provided.
        /// </summary>
        [JsonPropertyName("accountNumber")]
        public string AccountNumber { get; set; }

        /// <summary>
        /// Gets or sets the account holder's full name.
        /// </summary>
        [JsonPropertyName("accountHolder")]
        public string AccountHolder { get; set; }

        /// <summary>
        /// Gets or sets the current account balance in currency units.
        /// </summary>
        [JsonPropertyName("balance")]
        public decimal Balance { get; set; }

        /// <summary>
        /// Gets or sets the account creation date and time.
        /// </summary>
        [JsonPropertyName("createdDate")]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the last modification date and time.
        /// </summary>
        [JsonPropertyName("lastModifiedDate")]
        public DateTime LastModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the account is active.
        /// </summary>
        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the transaction history for this account.
        /// </summary>
        [JsonPropertyName("transactions")]
        public List<Transaction> Transactions { get; set; }

        /// <summary>
        /// Initializes a new instance of the Account class with default values.
        /// </summary>
        public Account()
        {
            Transactions = new List<Transaction>();
            CreatedDate = DateTime.Now;
            LastModifiedDate = DateTime.Now;
            IsActive = true;
        }

        /// <summary>
        /// Initializes a new instance of the Account class with specified account holder name.
        /// </summary>
        /// <param name="accountHolder">The name of the account holder</param>
        public Account(string accountHolder) : this()
        {
            AccountHolder = accountHolder;
            AccountNumber = GenerateAccountNumber();
        }

        /// <summary>
        /// Generates a unique account number using timestamp and random numbers.
        /// Format: ACCT-[8 random alphanumeric characters]
        /// </summary>
        /// <returns>A unique account number string</returns>
        private static string GenerateAccountNumber()
        {
            const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Random random = new Random();
            string randomPart = new string(Enumerable.Range(0, 8)
                .Select(_ => chars[random.Next(chars.Length)])
                .ToArray());
            return $"ACCT-{randomPart}";
        }

        /// <summary>
        /// Returns a string representation of the account including key details.
        /// </summary>
        /// <returns>Formatted account information</returns>
        public override string ToString()
        {
            return $"Account Number: {AccountNumber}, Holder: {AccountHolder}, Balance: ${Balance:F2}, Active: {IsActive}";
        }
    }
}
