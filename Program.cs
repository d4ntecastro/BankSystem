using System;
using System.Collections.Generic;
using BankSystem;

Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
Console.WriteLine("║     Bank Account Management System - Interactive Mode     ║");
Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

IAccountService accountService = new AccountService();
bool running = true;

while (running)
{
    Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
    Console.WriteLine("║                      MAIN MENU                           ║");
    Console.WriteLine("╠═══════════════════════════════════════════════════════════╣");
    Console.WriteLine("║  1. Create Account                                        ║");
    Console.WriteLine("║  2. View All Accounts                                     ║");
    Console.WriteLine("║  3. Deposit Money                                         ║");
    Console.WriteLine("║  4. Withdraw Money                                        ║");
    Console.WriteLine("║  5. Check Balance                                         ║");
    Console.WriteLine("║  6. View Transaction History                              ║");
    Console.WriteLine("║  7. Delete Account                                        ║");
    Console.WriteLine("║  8. View System Statistics                                ║");
    Console.WriteLine("║  9. Exit                                                  ║");
    Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
    Console.Write("\nEnter your choice (1-9): ");

    string choice = Console.ReadLine() ?? "";

    switch (choice)
    {
        case "1":
            CreateAccountMenu(accountService);
            break;
        case "2":
            ViewAllAccountsMenu(accountService);
            break;
        case "3":
            DepositMenu(accountService);
            break;
        case "4":
            WithdrawMenu(accountService);
            break;
        case "5":
            CheckBalanceMenu(accountService);
            break;
        case "6":
            ViewTransactionHistoryMenu(accountService);
            break;
        case "7":
            DeleteAccountMenu(accountService);
            break;
        case "8":
            ViewStatisticsMenu(accountService);
            break;
        case "9":
            Console.WriteLine("\n✓ Thank you for using Bank System. Goodbye!");
            running = false;
            break;
        default:
            Console.WriteLine("\n✗ Invalid choice. Please enter a number between 1 and 9.");
            break;
    }
}

// ============================================
// Menu Functions
// ============================================

static void CreateAccountMenu(IAccountService service)
{
    Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
    Console.WriteLine("║                    CREATE ACCOUNT                         ║");
    Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
    
    Console.Write("\nEnter account holder name: ");
    string name = Console.ReadLine() ?? "";

    if (string.IsNullOrWhiteSpace(name))
    {
        Console.WriteLine("✗ Error: Name cannot be empty!");
        return;
    }

    try
    {
        Account account = service.CreateAccount(name);
        Console.WriteLine($"\n✓ Account created successfully!");
        Console.WriteLine($"  Account Number: {account.AccountNumber}");
        Console.WriteLine($"  Holder: {account.AccountHolder}");
        Console.WriteLine($"  Initial Balance: ${account.Balance:F2}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n✗ Error: {ex.Message}");
    }
}

static void ViewAllAccountsMenu(IAccountService service)
{
    Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
    Console.WriteLine("║                    ALL ACCOUNTS                           ║");
    Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

    List<Account> accounts = service.GetAllAccounts();

    if (accounts.Count == 0)
    {
        Console.WriteLine("No accounts found.");
        return;
    }

    Console.WriteLine("┌─────────────────────────────────────────────────────────────────┐");
    Console.WriteLine("│ Account#         │ Holder              │ Balance    │ Status     │");
    Console.WriteLine("├─────────────────────────────────────────────────────────────────┤");

    foreach (var account in accounts)
    {
        string status = account.IsActive ? "Active" : "Deleted";
        Console.WriteLine($"│ {account.AccountNumber,-15} │ {account.AccountHolder,-19} │ ${account.Balance,9:F2} │ {status,-10} │");
    }

    Console.WriteLine("└─────────────────────────────────────────────────────────────────┘");
}

static void DepositMenu(IAccountService service)
{
    Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
    Console.WriteLine("║                    DEPOSIT MONEY                          ║");
    Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");

    Console.Write("\nEnter account number: ");
    string accountNumber = Console.ReadLine() ?? "";

    Console.Write("Enter amount to deposit: $");
    string amountStr = Console.ReadLine() ?? "";

    if (!decimal.TryParse(amountStr, out decimal amount))
    {
        Console.WriteLine("\n✗ Error: Invalid amount entered!");
        return;
    }

    try
    {
        Account updated = service.Deposit(accountNumber, amount);
        Console.WriteLine($"\n✓ Deposit successful!");
        Console.WriteLine($"  Account: {updated.AccountNumber}");
        Console.WriteLine($"  Deposited: ${amount:F2}");
        Console.WriteLine($"  New Balance: ${updated.Balance:F2}");
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"\n✗ Error: {ex.Message}");
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"\n✗ Error: {ex.Message}");
    }
}

static void WithdrawMenu(IAccountService service)
{
    Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
    Console.WriteLine("║                   WITHDRAW MONEY                          ║");
    Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");

    Console.Write("\nEnter account number: ");
    string accountNumber = Console.ReadLine() ?? "";

    Console.Write("Enter amount to withdraw: $");
    string amountStr = Console.ReadLine() ?? "";

    if (!decimal.TryParse(amountStr, out decimal amount))
    {
        Console.WriteLine("\n✗ Error: Invalid amount entered!");
        return;
    }

    try
    {
        Account updated = service.Withdraw(accountNumber, amount);
        Console.WriteLine($"\n✓ Withdrawal successful!");
        Console.WriteLine($"  Account: {updated.AccountNumber}");
        Console.WriteLine($"  Withdrawn: ${amount:F2}");
        Console.WriteLine($"  Remaining Balance: ${updated.Balance:F2}");
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"\n✗ Error: {ex.Message}");
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"\n✗ Error: {ex.Message}");
    }
}

static void CheckBalanceMenu(IAccountService service)
{
    Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
    Console.WriteLine("║                   CHECK BALANCE                           ║");
    Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");

    Console.Write("\nEnter account number: ");
    string accountNumber = Console.ReadLine() ?? "";

    try
    {
        decimal balance = service.GetBalance(accountNumber);
        Console.WriteLine($"\n✓ Balance for {accountNumber}: ${balance:F2}");
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"\n✗ Error: {ex.Message}");
    }
}

static void ViewTransactionHistoryMenu(IAccountService service)
{
    Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
    Console.WriteLine("║              TRANSACTION HISTORY                          ║");
    Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");

    Console.Write("\nEnter account number: ");
    string accountNumber = Console.ReadLine() ?? "";

    try
    {
        List<Transaction> transactions = service.GetTransactionHistory(accountNumber);

        if (transactions.Count == 0)
        {
            Console.WriteLine("\nNo transactions found.");
            return;
        }

        Console.WriteLine($"\nTransactions for {accountNumber}:\n");
        Console.WriteLine("┌────────────────────────────────────────────────────────────────────┐");
        Console.WriteLine("│ Timestamp            │ Type         │ Amount    │ Balance After │");
        Console.WriteLine("├────────────────────────────────────────────────────────────────────┤");

        foreach (var transaction in transactions)
        {
            string timestamp = transaction.Timestamp.ToString("yyyy-MM-dd HH:mm:ss");
            string type = transaction.Type.ToString().PadRight(12);
            Console.WriteLine($"│ {timestamp} │ {type} │ ${transaction.Amount,8:F2} │ ${transaction.BalanceAfter,12:F2} │");
        }

        Console.WriteLine("└────────────────────────────────────────────────────────────────────┘");
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"\n✗ Error: {ex.Message}");
    }
}

static void DeleteAccountMenu(IAccountService service)
{
    Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
    Console.WriteLine("║                   DELETE ACCOUNT                          ║");
    Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");

    Console.Write("\nEnter account number to delete: ");
    string accountNumber = Console.ReadLine() ?? "";

    Console.Write("Are you sure? (yes/no): ");
    string confirm = Console.ReadLine() ?? "";

    if (confirm.ToLower() != "yes")
    {
        Console.WriteLine("\n✓ Deletion cancelled.");
        return;
    }

    try
    {
        bool deleted = service.DeleteAccount(accountNumber);
        if (deleted)
        {
            Console.WriteLine($"\n✓ Account {accountNumber} has been deleted successfully.");
        }
        else
        {
            Console.WriteLine($"\n✗ Account {accountNumber} not found.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n✗ Error: {ex.Message}");
    }
}

static void ViewStatisticsMenu(IAccountService service)
{
    Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
    Console.WriteLine("║              SYSTEM STATISTICS                            ║");
    Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

    var accountService = (AccountService)service;

    int totalAccounts = accountService.GetAccountCount();
    int activeAccounts = accountService.GetActiveAccountCount();
    decimal totalBalance = accountService.GetTotalBalance();

    Console.WriteLine($"  Total Accounts:    {totalAccounts}");
    Console.WriteLine($"  Active Accounts:   {activeAccounts}");
    Console.WriteLine($"  Deleted Accounts:  {totalAccounts - activeAccounts}");
    Console.WriteLine($"  Total Balance:     ${totalBalance:F2}");
    Console.WriteLine($"  Data Location:     BankData/accounts.json");
}
