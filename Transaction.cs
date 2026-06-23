using System;
using System.Text.Json.Serialization;

namespace BankSystem
{
    /// <summary>
    /// Represents a single transaction in a bank account.
    /// Transactions are immutable records of deposits, withdrawals, and other account activities.
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// Enumeration of transaction types.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum TransactionType
        {
            /// <summary>Money added to the account</summary>
            Deposit,
            /// <summary>Money removed from the account</summary>
            Withdrawal,
            /// <summary>Account created</summary>
            AccountCreation
        }

        /// <summary>
        /// Gets or sets the unique transaction ID.
        /// </summary>
        [JsonPropertyName("transactionId")]
        public string TransactionId { get; set; }

        /// <summary>
        /// Gets or sets the type of transaction (Deposit, Withdrawal, AccountCreation).
        /// </summary>
        [JsonPropertyName("transactionType")]
        public TransactionType Type { get; set; }

        /// <summary>
        /// Gets or sets the transaction amount in currency units.
        /// </summary>
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the balance after this transaction was completed.
        /// </summary>
        [JsonPropertyName("balanceAfter")]
        public decimal BalanceAfter { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of when this transaction occurred.
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets a description or reason for the transaction.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// Initializes a new instance of the Transaction class.
        /// </summary>
        public Transaction()
        {
            Timestamp = DateTime.Now;
            TransactionId = Guid.NewGuid().ToString().Substring(0, 8);
        }

        /// <summary>
        /// Initializes a new instance of the Transaction class with specified parameters.
        /// </summary>
        /// <param name="type">The type of transaction</param>
        /// <param name="amount">The transaction amount</param>
        /// <param name="balanceAfter">The account balance after the transaction</param>
        /// <param name="description">Optional description of the transaction</param>
        public Transaction(TransactionType type, decimal amount, decimal balanceAfter, string description = "") : this()
        {
            Type = type;
            Amount = amount;
            BalanceAfter = balanceAfter;
            Description = description;
        }

        /// <summary>
        /// Returns a formatted string representation of the transaction.
        /// </summary>
        /// <returns>Formatted transaction details</returns>
        public override string ToString()
        {
            return $"[{Timestamp:yyyy-MM-dd HH:mm:ss}] {Type}: ${Amount:F2} | Balance: ${BalanceAfter:F2} | {Description}";
        }
    }
}
