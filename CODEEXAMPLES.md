# Code Examples & Common Patterns

## Usage Patterns and Code Snippets

### Basic Initialization

```csharp
using BankSystem;

// Create service instance
// Automatically loads any existing data from storage
IAccountService accountService = new AccountService();
```

---

## Account Operations

### Create an Account

```csharp
// Simple creation
Account account = accountService.CreateAccount("John Doe");
Console.WriteLine($"Created: {account.AccountNumber}");

// Multiple accounts
var alice = accountService.CreateAccount("Alice Johnson");
var bob = accountService.CreateAccount("Bob Smith");
var charlie = accountService.CreateAccount("Charlie Brown");
```

### Retrieve an Account

```csharp
// Get single account
Account account = accountService.GetAccount("ACCT-ABC12345");

// Get all accounts
List<Account> allAccounts = accountService.GetAllAccounts();

// Display all accounts
foreach (var acc in allAccounts)
{
    Console.WriteLine($"{acc.AccountNumber} | {acc.AccountHolder} | ${acc.Balance:F2}");
}
```

### Delete an Account

```csharp
// Mark as inactive (soft delete)
bool deleted = accountService.DeleteAccount("ACCT-ABC12345");

if (deleted)
{
    Console.WriteLine("Account deleted successfully");
}

// Account still exists but IsActive = false
// Operations on it will throw InvalidOperationException
```

---

## Financial Operations

### Deposit Funds

```csharp
// Simple deposit
Account updated = accountService.Deposit("ACCT-ABC12345", 500m);
Console.WriteLine($"New balance: ${updated.Balance:F2}");

// Multiple deposits
accountService.Deposit("ACCT-ABC12345", 100m);
accountService.Deposit("ACCT-ABC12345", 250m);
accountService.Deposit("ACCT-ABC12345", 75m);
// Total: $425 added

// Deposit with automatic persistence
// No manual save needed - happens automatically
```

### Withdraw Funds

```csharp
// Simple withdrawal
Account updated = accountService.Withdraw("ACCT-ABC12345", 150m);
Console.WriteLine($"Remaining balance: ${updated.Balance:F2}");

// Multiple withdrawals
try
{
    accountService.Withdraw("ACCT-ABC12345", 100m);
    accountService.Withdraw("ACCT-ABC12345", 50m);
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Withdrawal failed: {ex.Message}");
}

// Withdraw entire balance
decimal balance = accountService.GetBalance("ACCT-ABC12345");
accountService.Withdraw("ACCT-ABC12345", balance);
// Balance is now 0
```

### Check Balance

```csharp
// Get balance for specific account
decimal balance = accountService.GetBalance("ACCT-ABC12345");
Console.WriteLine($"Balance: ${balance:F2}");

// Safe check with try-catch
try
{
    decimal bal = accountService.GetBalance("INVALID-123");
}
catch (InvalidOperationException)
{
    Console.WriteLine("Account not found");
}
```

---

## Transaction Management

### View Transaction History

```csharp
// Get all transactions for an account
List<Transaction> transactions = accountService.GetTransactionHistory("ACCT-ABC12345");

// Display formatted
foreach (var t in transactions)
{
    Console.WriteLine($"{t.Timestamp:yyyy-MM-dd HH:mm:ss} | " +
                      $"{t.Type,-15} | " +
                      $"${t.Amount,10:F2} | " +
                      $"Balance: ${t.BalanceAfter:F2}");
}

// Count transactions
int transactionCount = transactions.Count;
Console.WriteLine($"Total transactions: {transactionCount}");

// Find specific transaction type
var deposits = transactions.Where(t => t.Type == Transaction.TransactionType.Deposit).ToList();
var withdrawals = transactions.Where(t => t.Type == Transaction.TransactionType.Withdrawal).ToList();
```

### Analyze Transaction History

```csharp
// Total deposits
decimal totalDeposits = transactions
    .Where(t => t.Type == Transaction.TransactionType.Deposit)
    .Sum(t => t.Amount);

// Total withdrawals
decimal totalWithdrawals = transactions
    .Where(t => t.Type == Transaction.TransactionType.Withdrawal)
    .Sum(t => t.Amount);

// Average transaction amount
decimal avgTransaction = transactions
    .Where(t => t.Type != Transaction.TransactionType.AccountCreation)
    .Average(t => t.Amount);

Console.WriteLine($"Total Deposits: ${totalDeposits:F2}");
Console.WriteLine($"Total Withdrawals: ${totalWithdrawals:F2}");
Console.WriteLine($"Average Transaction: ${avgTransaction:F2}");
```

---

## System Statistics & Reporting

### Get System Statistics

```csharp
// Cast to AccountService to access utility methods
var service = (AccountService)accountService;

int totalAccounts = service.GetAccountCount();
int activeAccounts = service.GetActiveAccountCount();
decimal totalBalance = service.GetTotalBalance();

Console.WriteLine($"Total Accounts: {totalAccounts}");
Console.WriteLine($"Active Accounts: {activeAccounts}");
Console.WriteLine($"Total Balance: ${totalBalance:F2}");
```

### Generate Account Report

```csharp
var allAccounts = accountService.GetAllAccounts();

Console.WriteLine("╔═══════════════════════════════════════════════════╗");
Console.WriteLine("║           ACCOUNT SUMMARY REPORT                  ║");
Console.WriteLine("╚═══════════════════════════════════════════════════╝\n");

Console.WriteLine($"{'Account #',-18} {'Holder',-20} {'Balance',-12} {'Status',-8}");
Console.WriteLine("──────────────────────────────────────────────────");

decimal runningTotal = 0m;
foreach (var account in allAccounts)
{
    if (account.IsActive)
    {
        Console.WriteLine($"{account.AccountNumber,-18} {account.AccountHolder,-20} " +
                          $"${account.Balance,10:F2} {'Active',-8}");
        runningTotal += account.Balance;
    }
}

Console.WriteLine("──────────────────────────────────────────────────");
Console.WriteLine($"{'TOTAL',-18} {'',-20} ${runningTotal,10:F2}");
```

---

## Error Handling Patterns

### Safe Deposit/Withdraw with Error Handling

```csharp
public bool TryDeposit(IAccountService service, string accountNumber, decimal amount)
{
    try
    {
        service.Deposit(accountNumber, amount);
        Console.WriteLine($"✓ Deposited ${amount:F2}");
        return true;
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"✗ Deposit failed: {ex.Message}");
        return false;
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"✗ Account error: {ex.Message}");
        return false;
    }
}

public bool TryWithdraw(IAccountService service, string accountNumber, decimal amount)
{
    try
    {
        service.Withdraw(accountNumber, amount);
        Console.WriteLine($"✓ Withdrew ${amount:F2}");
        return true;
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"✗ Withdrawal failed: {ex.Message}");
        return false;
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"✗ Account error: {ex.Message}");
        return false;
    }
}

// Usage
TryDeposit(accountService, accountNumber, 500m);
TryWithdraw(accountService, accountNumber, 1000m);
```

### Safe Account Creation

```csharp
public Account TryCreateAccount(IAccountService service, string accountHolder)
{
    if (string.IsNullOrWhiteSpace(accountHolder))
    {
        Console.WriteLine("✗ Account holder name is required");
        return null;
    }

    try
    {
        Account account = service.CreateAccount(accountHolder);
        Console.WriteLine($"✓ Account created: {account.AccountNumber}");
        return account;
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"✗ Failed to create account: {ex.Message}");
        return null;
    }
}

// Usage
var account = TryCreateAccount(accountService, "John Doe");
if (account != null)
{
    // Safe to use account
}
```

---

## Data Persistence Patterns

### Manual Save (Usually Not Needed)

```csharp
// Automatic save happens after every operation
// But you can manually save if needed
bool saved = accountService.SaveData();
if (saved)
{
    Console.WriteLine("Data saved successfully");
}
```

### Reload from Storage

```csharp
// Useful if external changes made to data file
bool loaded = accountService.LoadData();
if (loaded)
{
    Console.WriteLine("Data reloaded from storage");
}
```

### Backup Data

```csharp
// Simple backup by copying accounts
List<Account> backup = accountService.GetAllAccounts();
Console.WriteLine($"Backup created: {backup.Count} accounts");

// Could be serialized separately or logged
```

---

## Advanced Patterns

### Process Multiple Accounts

```csharp
// Create and fund multiple accounts
var accounts = new List<Account>();
string[] names = { "Alice", "Bob", "Charlie", "Diana", "Eve" };
decimal[] initialDeposits = { 1000m, 2000m, 1500m, 3000m, 2500m };

for (int i = 0; i < names.Length; i++)
{
    var account = accountService.CreateAccount(names[i]);
    accountService.Deposit(account.AccountNumber, initialDeposits[i]);
    accounts.Add(account);
}

Console.WriteLine($"Created {accounts.Count} accounts");
```

### Distribute Funds

```csharp
// Transfer-like operation (without direct transfer method)
decimal transferAmount = 100m;

// From Account A, withdraw
accountService.Withdraw(fromAccount, transferAmount);

// To Account B, deposit
accountService.Deposit(toAccount, transferAmount);

// Transaction history shows both operations
```

### Find Account by Holder Name

```csharp
public Account FindAccountByHolder(IAccountService service, string name)
{
    var accounts = service.GetAllAccounts();
    return accounts.FirstOrDefault(a => 
        a.AccountHolder.Equals(name, StringComparison.OrdinalIgnoreCase));
}

// Usage
var aliceAccount = FindAccountByHolder(accountService, "Alice Johnson");
if (aliceAccount != null)
{
    Console.WriteLine($"Found: {aliceAccount.AccountNumber}");
}
```

### Calculate Account Statistics

```csharp
public class AccountStats
{
    public string AccountNumber { get; set; }
    public string Holder { get; set; }
    public decimal CurrentBalance { get; set; }
    public int TransactionCount { get; set; }
    public decimal TotalDeposited { get; set; }
    public decimal TotalWithdrawn { get; set; }
}

public AccountStats GetAccountStats(IAccountService service, string accountNumber)
{
    var account = service.GetAccount(accountNumber);
    if (account == null) return null;

    var transactions = service.GetTransactionHistory(accountNumber);
    
    var stats = new AccountStats
    {
        AccountNumber = accountNumber,
        Holder = account.AccountHolder,
        CurrentBalance = account.Balance,
        TransactionCount = transactions.Count,
        TotalDeposited = transactions
            .Where(t => t.Type == Transaction.TransactionType.Deposit)
            .Sum(t => t.Amount),
        TotalWithdrawn = transactions
            .Where(t => t.Type == Transaction.TransactionType.Withdrawal)
            .Sum(t => t.Amount)
    };

    return stats;
}

// Usage
var stats = GetAccountStats(accountService, "ACCT-ABC12345");
Console.WriteLine($"Account: {stats.Holder}");
Console.WriteLine($"Balance: ${stats.CurrentBalance:F2}");
Console.WriteLine($"Total Transactions: {stats.TransactionCount}");
Console.WriteLine($"Total In: ${stats.TotalDeposited:F2}");
Console.WriteLine($"Total Out: ${stats.TotalWithdrawn:F2}");
```

### Validate Account Health

```csharp
public bool IsAccountHealthy(IAccountService service, string accountNumber)
{
    try
    {
        var account = service.GetAccount(accountNumber);
        if (account == null) return false;
        if (!account.IsActive) return false;
        if (account.Balance < 0) return false; // Should never happen
        if (account.Transactions.Count == 0) return false;
        
        return true;
    }
    catch
    {
        return false;
    }
}

// Usage
if (IsAccountHealthy(accountService, accountNumber))
{
    Console.WriteLine("✓ Account is healthy");
}
else
{
    Console.WriteLine("✗ Account has issues");
}
```

---

## Testing Patterns

### Test Setup Pattern

```csharp
[SetUp]
public void Setup()
{
    service = new AccountService();
    service.ClearAllData(); // Start fresh
}

[TearDown]
public void Teardown()
{
    service.ClearAllData(); // Cleanup
}

[Test]
public void MyTest()
{
    // Arrange
    Account account = service.CreateAccount("Test User");
    
    // Act
    service.Deposit(account.AccountNumber, 500m);
    
    // Assert
    Assert.AreEqual(500m, service.GetBalance(account.AccountNumber));
}
```

---

## Complete Example: Mini ATM

```csharp
class MiniATM
{
    private IAccountService service = new AccountService();
    private Account currentAccount;

    public void Run()
    {
        while (true)
        {
            Console.WriteLine("\n=== ATM Menu ===");
            Console.WriteLine("1. Create Account");
            Console.WriteLine("2. Select Account");
            Console.WriteLine("3. Deposit");
            Console.WriteLine("4. Withdraw");
            Console.WriteLine("5. Check Balance");
            Console.WriteLine("6. View Transactions");
            Console.WriteLine("7. Exit");
            Console.Write("Choice: ");

            switch (Console.ReadLine())
            {
                case "1":
                    CreateAccount();
                    break;
                case "2":
                    SelectAccount();
                    break;
                case "3":
                    Deposit();
                    break;
                case "4":
                    Withdraw();
                    break;
                case "5":
                    CheckBalance();
                    break;
                case "6":
                    ViewTransactions();
                    break;
                case "7":
                    return;
            }
        }
    }

    void CreateAccount()
    {
        Console.Write("Name: ");
        string name = Console.ReadLine();
        currentAccount = service.CreateAccount(name);
        Console.WriteLine($"Account created: {currentAccount.AccountNumber}");
    }

    void SelectAccount()
    {
        Console.Write("Account number: ");
        string number = Console.ReadLine();
        currentAccount = service.GetAccount(number);
        if (currentAccount != null)
            Console.WriteLine("Account selected");
        else
            Console.WriteLine("Account not found");
    }

    void Deposit()
    {
        if (currentAccount == null)
        {
            Console.WriteLine("No account selected");
            return;
        }
        Console.Write("Amount: ");
        if (decimal.TryParse(Console.ReadLine(), out decimal amount))
        {
            try
            {
                service.Deposit(currentAccount.AccountNumber, amount);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    void Withdraw()
    {
        if (currentAccount == null)
        {
            Console.WriteLine("No account selected");
            return;
        }
        Console.Write("Amount: ");
        if (decimal.TryParse(Console.ReadLine(), out decimal amount))
        {
            try
            {
                service.Withdraw(currentAccount.AccountNumber, amount);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    void CheckBalance()
    {
        if (currentAccount == null)
        {
            Console.WriteLine("No account selected");
            return;
        }
        decimal balance = service.GetBalance(currentAccount.AccountNumber);
        Console.WriteLine($"Balance: ${balance:F2}");
    }

    void ViewTransactions()
    {
        if (currentAccount == null)
        {
            Console.WriteLine("No account selected");
            return;
        }
        var transactions = service.GetTransactionHistory(currentAccount.AccountNumber);
        foreach (var t in transactions)
            Console.WriteLine(t);
    }
}

// Usage
var atm = new MiniATM();
atm.Run();
```

---

**That's it!** These patterns cover most common use cases.
For more details, see README.md and ARCHITECTURE.md
