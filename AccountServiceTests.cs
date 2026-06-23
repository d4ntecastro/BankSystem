using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using BankSystem;

namespace BankSystem.Tests
{
    /// <summary>
    /// Unit tests for the AccountService class.
    /// Tests all core functionality including account creation, deposits, withdrawals,
    /// data persistence, and error handling.
    /// </summary>
    [TestFixture]
    public class AccountServiceTests
    {
        private AccountService _accountService;

        /// <summary>
        /// Setup method that runs before each test.
        /// Initializes a fresh AccountService and clears any existing data.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _accountService = new AccountService();
            _accountService.ClearAllData();
        }

        /// <summary>
        /// Cleanup method that runs after each test.
        /// Ensures data is cleaned up to avoid test pollution.
        /// </summary>
        [TearDown]
        public void Teardown()
        {
            _accountService.ClearAllData();
        }

        // ============================================
        // Account Creation Tests
        // ============================================

        /// <summary>
        /// Test: Creating an account with a valid account holder name should succeed.
        /// </summary>
        [Test]
        public void CreateAccount_WithValidName_ShouldSucceed()
        {
            // Arrange & Act
            Account account = _accountService.CreateAccount("John Doe");

            // Assert
            Assert.IsNotNull(account);
            Assert.AreEqual("John Doe", account.AccountHolder);
            Assert.AreEqual(0m, account.Balance);
            Assert.IsTrue(account.IsActive);
            Assert.IsNotNull(account.AccountNumber);
            Assert.IsTrue(account.Transactions.Count > 0); // Should have creation transaction
        }

        /// <summary>
        /// Test: Creating an account with null name should throw ArgumentException.
        /// </summary>
        [Test]
        public void CreateAccount_WithNullName_ShouldThrowArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => _accountService.CreateAccount(null));
        }

        /// <summary>
        /// Test: Creating an account with empty name should throw ArgumentException.
        /// </summary>
        [Test]
        public void CreateAccount_WithEmptyName_ShouldThrowArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => _accountService.CreateAccount(""));
        }

        /// <summary>
        /// Test: Creating multiple accounts should generate unique account numbers.
        /// </summary>
        [Test]
        public void CreateAccount_Multiple_ShouldHaveUniqueAccountNumbers()
        {
            // Arrange & Act
            Account account1 = _accountService.CreateAccount("John Doe");
            Account account2 = _accountService.CreateAccount("Jane Smith");

            // Assert
            Assert.AreNotEqual(account1.AccountNumber, account2.AccountNumber);
            Assert.AreEqual(2, _accountService.GetAllAccounts().Count);
        }

        // ============================================
        // Deposit Tests
        // ============================================

        /// <summary>
        /// Test: Depositing a valid amount should increase balance.
        /// </summary>
        [Test]
        public void Deposit_ValidAmount_ShouldIncreaseBalance()
        {
            // Arrange
            Account account = _accountService.CreateAccount("John Doe");
            string accountNumber = account.AccountNumber;

            // Act
            Account updated = _accountService.Deposit(accountNumber, 100m);

            // Assert
            Assert.AreEqual(100m, updated.Balance);
            Assert.AreEqual(2, updated.Transactions.Count); // Creation + Deposit
            Assert.AreEqual(Transaction.TransactionType.Deposit, updated.Transactions[1].Type);
        }

        /// <summary>
        /// Test: Depositing zero amount should throw ArgumentException.
        /// </summary>
        [Test]
        public void Deposit_ZeroAmount_ShouldThrowArgumentException()
        {
            // Arrange
            Account account = _accountService.CreateAccount("John Doe");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _accountService.Deposit(account.AccountNumber, 0m));
        }

        /// <summary>
        /// Test: Depositing negative amount should throw ArgumentException.
        /// </summary>
        [Test]
        public void Deposit_NegativeAmount_ShouldThrowArgumentException()
        {
            // Arrange
            Account account = _accountService.CreateAccount("John Doe");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _accountService.Deposit(account.AccountNumber, -100m));
        }

        /// <summary>
        /// Test: Depositing to non-existent account should throw InvalidOperationException.
        /// </summary>
        [Test]
        public void Deposit_NonExistentAccount_ShouldThrowInvalidOperationException()
        {
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _accountService.Deposit("INVALID-123", 100m));
        }

        /// <summary>
        /// Test: Multiple deposits should accumulate correctly.
        /// </summary>
        [Test]
        public void Deposit_Multiple_ShouldAccumulate()
        {
            // Arrange
            Account account = _accountService.CreateAccount("John Doe");

            // Act
            _accountService.Deposit(account.AccountNumber, 100m);
            _accountService.Deposit(account.AccountNumber, 50m);
            Account final = _accountService.Deposit(account.AccountNumber, 25m);

            // Assert
            Assert.AreEqual(175m, final.Balance);
            Assert.AreEqual(4, final.Transactions.Count); // 1 creation + 3 deposits
        }

        // ============================================
        // Withdrawal Tests
        // ============================================

        /// <summary>
        /// Test: Withdrawing valid amount should decrease balance.
        /// </summary>
        [Test]
        public void Withdraw_ValidAmount_ShouldDecreaseBalance()
        {
            // Arrange
            Account account = _accountService.CreateAccount("John Doe");
            _accountService.Deposit(account.AccountNumber, 500m);

            // Act
            Account updated = _accountService.Withdraw(account.AccountNumber, 100m);

            // Assert
            Assert.AreEqual(400m, updated.Balance);
            Assert.AreEqual(Transaction.TransactionType.Withdrawal, updated.Transactions[2].Type);
        }

        /// <summary>
        /// Test: Withdrawing zero amount should throw ArgumentException.
        /// </summary>
        [Test]
        public void Withdraw_ZeroAmount_ShouldThrowArgumentException()
        {
            // Arrange
            Account account = _accountService.CreateAccount("John Doe");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _accountService.Withdraw(account.AccountNumber, 0m));
        }

        /// <summary>
        /// Test: Withdrawing negative amount should throw ArgumentException.
        /// </summary>
        [Test]
        public void Withdraw_NegativeAmount_ShouldThrowArgumentException()
        {
            // Arrange
            Account account = _accountService.CreateAccount("John Doe");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _accountService.Withdraw(account.AccountNumber, -100m));
        }

        /// <summary>
        /// Test: Withdrawing more than balance should throw ArgumentException.
        /// </summary>
        [Test]
        public void Withdraw_InsufficientFunds_ShouldThrowArgumentException()
        {
            // Arrange
            Account account = _accountService.CreateAccount("John Doe");
            _accountService.Deposit(account.AccountNumber, 100m);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _accountService.Withdraw(account.AccountNumber, 150m));
        }

        /// <summary>
        /// Test: Withdrawing entire balance should result in zero balance.
        /// </summary>
        [Test]
        public void Withdraw_EntireBalance_ShouldResultInZeroBalance()
        {
            // Arrange
            Account account = _accountService.CreateAccount("John Doe");
            _accountService.Deposit(account.AccountNumber, 250m);

            // Act
            Account updated = _accountService.Withdraw(account.AccountNumber, 250m);

            // Assert
            Assert.AreEqual(0m, updated.Balance);
        }

        /// <summary>
        /// Test: Withdrawing from non-existent account should throw InvalidOperationException.
        /// </summary>
        [Test]
        public void Withdraw_NonExistentAccount_ShouldThrowInvalidOperationException()
        {
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _accountService.Withdraw("INVALID-123", 100m));
        }

        // ============================================
        // Account Retrieval Tests
        // ============================================

        /// <summary>
        /// Test: GetAccount should return the correct account.
        /// </summary>
        [Test]
        public void GetAccount_ExistingAccount_ShouldReturnAccount()
        {
            // Arrange
            Account created = _accountService.CreateAccount("John Doe");

            // Act
            Account retrieved = _accountService.GetAccount(created.AccountNumber);

            // Assert
            Assert.IsNotNull(retrieved);
            Assert.AreEqual(created.AccountNumber, retrieved.AccountNumber);
        }

        /// <summary>
        /// Test: GetAccount for non-existent account should return null.
        /// </summary>
        [Test]
        public void GetAccount_NonExistentAccount_ShouldReturnNull()
        {
            // Act
            Account retrieved = _accountService.GetAccount("INVALID-123");

            // Assert
            Assert.IsNull(retrieved);
        }

        /// <summary>
        /// Test: GetAllAccounts should return all created accounts.
        /// </summary>
        [Test]
        public void GetAllAccounts_MultipleAccounts_ShouldReturnAll()
        {
            // Arrange
            _accountService.CreateAccount("John Doe");
            _accountService.CreateAccount("Jane Smith");
            _accountService.CreateAccount("Bob Johnson");

            // Act
            List<Account> accounts = _accountService.GetAllAccounts();

            // Assert
            Assert.AreEqual(3, accounts.Count);
        }

        /// <summary>
        /// Test: GetBalance should return correct balance.
        /// </summary>
        [Test]
        public void GetBalance_ExistingAccount_ShouldReturnBalance()
        {
            // Arrange
            Account account = _accountService.CreateAccount("John Doe");
            _accountService.Deposit(account.AccountNumber, 500m);

            // Act
            decimal balance = _accountService.GetBalance(account.AccountNumber);

            // Assert
            Assert.AreEqual(500m, balance);
        }

        /// <summary>
        /// Test: GetBalance for non-existent account should throw InvalidOperationException.
        /// </summary>
        [Test]
        public void GetBalance_NonExistentAccount_ShouldThrowInvalidOperationException()
        {
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _accountService.GetBalance("INVALID-123"));
        }

        // ============================================
        // Transaction History Tests
        // ============================================

        /// <summary>
        /// Test: GetTransactionHistory should return all transactions for an account.
        /// </summary>
        [Test]
        public void GetTransactionHistory_MultipleTransactions_ShouldReturnAll()
        {
            // Arrange
            Account account = _accountService.CreateAccount("John Doe");
            _accountService.Deposit(account.AccountNumber, 100m);
            _accountService.Deposit(account.AccountNumber, 50m);
            _accountService.Withdraw(account.AccountNumber, 30m);

            // Act
            List<Transaction> history = _accountService.GetTransactionHistory(account.AccountNumber);

            // Assert
            Assert.AreEqual(4, history.Count); // 1 creation + 2 deposits + 1 withdrawal
            Assert.AreEqual(Transaction.TransactionType.AccountCreation, history[0].Type);
            Assert.AreEqual(Transaction.TransactionType.Deposit, history[1].Type);
            Assert.AreEqual(Transaction.TransactionType.Withdrawal, history[3].Type);
        }

        // ============================================
        // Account Deletion Tests
        // ============================================

        /// <summary>
        /// Test: DeleteAccount should mark account as inactive.
        /// </summary>
        [Test]
        public void DeleteAccount_ExistingAccount_ShouldMarkInactive()
        {
            // Arrange
            Account account = _accountService.CreateAccount("John Doe");

            // Act
            bool result = _accountService.DeleteAccount(account.AccountNumber);
            Account deleted = _accountService.GetAccount(account.AccountNumber);

            // Assert
            Assert.IsTrue(result);
            Assert.IsFalse(deleted.IsActive);
        }

        /// <summary>
        /// Test: Operations on inactive account should throw InvalidOperationException.
        /// </summary>
        [Test]
        public void DeleteAccount_OperationsOnDeletedAccount_ShouldThrow()
        {
            // Arrange
            Account account = _accountService.CreateAccount("John Doe");
            _accountService.DeleteAccount(account.AccountNumber);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _accountService.Deposit(account.AccountNumber, 100m));
            Assert.Throws<InvalidOperationException>(() => _accountService.Withdraw(account.AccountNumber, 50m));
        }

        // ============================================
        // Data Persistence Tests
        // ============================================

        /// <summary>
        /// Test: SaveData should persist account data to storage.
        /// </summary>
        [Test]
        public void SaveData_CreatedAccounts_ShouldPersist()
        {
            // Arrange
            Account account = _accountService.CreateAccount("John Doe");
            _accountService.Deposit(account.AccountNumber, 500m);

            // Act
            bool result = _accountService.SaveData();

            // Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Test: LoadData should restore accounts from storage.
        /// </summary>
        [Test]
        public void LoadData_PreviouslySavedData_ShouldRestore()
        {
            // Arrange
            Account account = _accountService.CreateAccount("John Doe");
            string accountNumber = account.AccountNumber;
            _accountService.Deposit(account.AccountNumber, 500m);

            // Create new service instance to simulate restart
            AccountService newService = new AccountService();

            // Act
            List<Account> loadedAccounts = newService.GetAllAccounts();

            // Assert
            Assert.AreEqual(1, loadedAccounts.Count);
            Assert.AreEqual("John Doe", loadedAccounts[0].AccountHolder);
            Assert.AreEqual(500m, loadedAccounts[0].Balance);
        }

        // ============================================
        // Utility Method Tests
        // ============================================

        /// <summary>
        /// Test: GetAccountCount should return correct count.
        /// </summary>
        [Test]
        public void GetAccountCount_MultipleAccounts_ShouldReturnCorrectCount()
        {
            // Arrange
            _accountService.CreateAccount("John Doe");
            _accountService.CreateAccount("Jane Smith");
            _accountService.CreateAccount("Bob Johnson");

            // Act
            int count = _accountService.GetAccountCount();

            // Assert
            Assert.AreEqual(3, count);
        }

        /// <summary>
        /// Test: GetTotalBalance should sum all active accounts.
        /// </summary>
        [Test]
        public void GetTotalBalance_MultipleAccounts_ShouldReturnSum()
        {
            // Arrange
            Account account1 = _accountService.CreateAccount("John Doe");
            Account account2 = _accountService.CreateAccount("Jane Smith");
            _accountService.Deposit(account1.AccountNumber, 100m);
            _accountService.Deposit(account2.AccountNumber, 200m);

            // Act
            decimal total = _accountService.GetTotalBalance();

            // Assert
            Assert.AreEqual(300m, total);
        }

        /// <summary>
        /// Test: GetTotalBalance should exclude inactive accounts.
        /// </summary>
        [Test]
        public void GetTotalBalance_WithInactiveAccounts_ShouldExcludeThem()
        {
            // Arrange
            Account account1 = _accountService.CreateAccount("John Doe");
            Account account2 = _accountService.CreateAccount("Jane Smith");
            _accountService.Deposit(account1.AccountNumber, 100m);
            _accountService.Deposit(account2.AccountNumber, 200m);
            _accountService.DeleteAccount(account1.AccountNumber);

            // Act
            decimal total = _accountService.GetTotalBalance();

            // Assert
            Assert.AreEqual(200m, total);
        }
    }
}
