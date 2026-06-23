# Quick Start Guide

Get up and running in 5 minutes!

## 1. Prerequisites Check

**Required:**
- .NET 7.0 or later
- Command line terminal

**Check your .NET version:**
```bash
dotnet --version
```

Should output something like: `7.0.0` or higher

## 2. Extract & Navigate

```bash
# Navigate to project directory
cd BankSystem
```

## 3. Build the Project

```bash
# Restore NuGet packages
dotnet restore

# Build the project
dotnet build
```

**Expected Output:**
```
Build succeeded.
```

## 4. Run the Demo

```bash
dotnet run --project BankSystem.csproj
```

**You should see:**
```
╔══════════════════════════════════════════════════════════╗
║        Bank Account Management System - Demo              ║
╚══════════════════════════════════════════════════════════╝

>>> Creating accounts...
✓ Account created successfully. Account Number: ACCT-XXXXXX
✓ Account created successfully. Account Number: ACCT-XXXXXX
...
```

## 5. Run Unit Tests

```bash
dotnet test
```

**Expected Output:**
```
Build started...
Build succeeded.

All tests passed! 35 passed, 0 failed
```

## 6. Verify Data Persistence

Check that data was saved:
```bash
# On Windows
type BankData\accounts.json

# On macOS/Linux
cat BankData/accounts.json
```

You should see JSON with account data.

---

## Quick Code Example

Create a file `MyDemo.cs` in the BankSystem directory:

```csharp
using System;
using BankSystem;

class MyDemo
{
    static void Main()
    {
        // Create service
        IAccountService service = new AccountService();
        
        // Create account
        Account account = service.CreateAccount("Your Name");
        Console.WriteLine($"Account: {account.AccountNumber}");
        
        // Deposit
        service.Deposit(account.AccountNumber, 1000);
        
        // Check balance
        decimal balance = service.GetBalance(account.AccountNumber);
        Console.WriteLine($"Balance: ${balance:F2}");
        
        // Withdraw
        service.Withdraw(account.AccountNumber, 250);
        Console.WriteLine($"After withdrawal: ${service.GetBalance(account.AccountNumber):F2}");
    }
}
```

Run it:
```bash
dotnet run MyDemo.cs
```

---

## Common Tasks

### Create Multiple Accounts and Perform Operations

```csharp
var service = new AccountService();

// Create accounts
var alice = service.CreateAccount("Alice");
var bob = service.CreateAccount("Bob");

// Operations
service.Deposit(alice.AccountNumber, 500);
service.Deposit(bob.AccountNumber, 300);

// Check balances
Console.WriteLine($"Alice: ${service.GetBalance(alice.AccountNumber):F2}");
Console.WriteLine($"Bob: ${service.GetBalance(bob.AccountNumber):F2}");

// Withdraw
service.Withdraw(alice.AccountNumber, 100);

// View transaction history
var transactions = service.GetTransactionHistory(alice.AccountNumber);
foreach (var t in transactions)
{
    Console.WriteLine(t);
}
```

### View All Accounts

```csharp
var service = new AccountService();
var accounts = service.GetAllAccounts();

foreach (var account in accounts)
{
    Console.WriteLine($"{account.AccountNumber}: {account.AccountHolder} - ${account.Balance:F2}");
}
```

### Handle Errors

```csharp
try
{
    service.Withdraw(account.AccountNumber, -100); // Invalid!
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}

try
{
    service.GetBalance("FAKE-123"); // Non-existent
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
```

---

## Project Structure Quick Reference

```
BankSystem/
├── Account.cs              ← Account model
├── Transaction.cs          ← Transaction record
├── IAccountService.cs      ← Interface (ADT)
├── AccountService.cs       ← Implementation
├── DataPersistence.cs      ← File storage
├── AccountServiceTests.cs  ← Unit tests
├── Program.cs              ← Demo application
├── BankSystem.csproj       ← Project file
├── README.md               ← Full documentation
├── ARCHITECTURE.md         ← Design details
└── BankData/               ← Data folder (created at runtime)
    └── accounts.json       ← Saved data
```

---

## File Descriptions

| File | Purpose | Key Classes |
|------|---------|------------|
| Account.cs | Account data model | `Account` |
| Transaction.cs | Transaction record | `Transaction`, `TransactionType` |
| IAccountService.cs | Service interface (ADT) | `IAccountService` |
| AccountService.cs | Core business logic | `AccountService` |
| DataPersistence.cs | File-based storage | `DataPersistence` |
| Program.cs | Demo application | Demo code examples |
| AccountServiceTests.cs | Unit tests | 35+ test methods |

---

## Testing Quick Commands

```bash
# Run all tests
dotnet test

# Run tests with details
dotnet test --verbosity=detailed

# Run specific test
dotnet test --filter "CreateAccount"

# Count tests
dotnet test --verbosity=quiet
```

---

## Troubleshooting

### Build fails
```bash
dotnet clean
dotnet restore
dotnet build
```

### Tests fail
- Ensure .NET 7.0+ is installed
- Run `dotnet restore`
- Check NUnit package is installed

### Data not saving
- Check `BankData/` directory exists
- Verify write permissions
- Run from project root directory

### Can't find accounts.json
- It's created automatically on first run
- Located in `BankData/accounts.json`
- Check if you have write permissions

---

## Next Steps

1. ✅ Run the demo: `dotnet run`
2. ✅ Run tests: `dotnet test`
3. ✅ Read README.md for full documentation
4. ✅ Check ARCHITECTURE.md for design details
5. ✅ Modify Program.cs to try your own operations
6. ✅ Add more test cases for practice

---

## Features Summary

✓ Create bank accounts with unique numbers
✓ Deposit and withdraw funds with validation
✓ Automatic transaction recording
✓ Persistent JSON-based storage
✓ Complete transaction history
✓ System statistics (total balance, account count)
✓ Error handling and validation
✓ 35+ comprehensive unit tests
✓ Professional documentation

---

## Key Design Patterns

- **Abstract Data Type (ADT)**: IAccountService interface
- **Service Pattern**: AccountService implementation
- **Collections**: List<T> for accounts and transactions
- **Persistence**: DataPersistence abstraction
- **Validation**: Multi-layer input checking

---

**Ready to go!** 🚀

For detailed documentation, see README.md
For architecture details, see ARCHITECTURE.md
