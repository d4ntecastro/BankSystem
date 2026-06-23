# Architecture & Design Documentation

## System Architecture Overview

```
┌─────────────────────────────────────────────────────────────┐
│                    Application Layer                         │
│                      (Program.cs)                            │
│              Demo & User Interface                           │
└────────────────┬────────────────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────────────────┐
│                  Service Layer                              │
│                 (IAccountService)                            │
│              Abstract Data Type (ADT)                        │
│          Defines contract for all operations                 │
└────────────────┬────────────────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────────────────┐
│              Implementation Layer                            │
│                 (AccountService)                             │
│          Business Logic & Operations                         │
│    - Create accounts                                         │
│    - Process deposits/withdrawals                            │
│    - Manage transactions                                     │
│    - Validate operations                                     │
└────────────────┬────────────────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────────────────┐
│                    Model Layer                               │
│              Account & Transaction Classes                   │
│          Data structures and validation                      │
└────────────────┬────────────────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────────────────┐
│              Persistence Layer                               │
│                (DataPersistence)                             │
│          File-based JSON storage                             │
└─────────────────────────────────────────────────────────────┘
```

## Design Patterns Used

### 1. **Abstract Data Type (ADT) Pattern**
The `IAccountService` interface defines the abstract data type for account operations without specifying implementation details.

```
┌──────────────────────────┐
│   IAccountService (ADT)  │
│  ┌────────────────────┐  │
│  │ CreateAccount()    │  │
│  │ Deposit()          │  │
│  │ Withdraw()         │  │
│  │ GetAllAccounts()   │  │
│  │ GetBalance()       │  │
│  │ GetTransHistory()  │  │
│  │ DeleteAccount()    │  │
│  │ SaveData()         │  │
│  │ LoadData()         │  │
│  └────────────────────┘  │
└────────────┬─────────────┘
             │
             │ implements
             │
┌────────────▼──────────────┐
│  AccountService           │
│  (Concrete Implementation)│
│  ┌────────────────────┐   │
│  │ - List<Account>    │   │
│  │ - DataPersistence  │   │
│  └────────────────────┘   │
└───────────────────────────┘
```

**Benefits:**
- Separates interface from implementation
- Allows easy testing (can mock the interface)
- Enables switching implementations without affecting clients
- Defines clear contract for operations

### 2. **Service Pattern**
`AccountService` encapsulates business logic and coordinates between models and persistence.

```
AccountService Responsibilities:
├── Account Management
│   ├── CreateAccount()
│   ├── GetAccount()
│   ├── DeleteAccount()
│   └── GetAllAccounts()
├── Financial Operations
│   ├── Deposit()
│   ├── Withdraw()
│   └── GetBalance()
├── Transaction Management
│   ├── Transaction Recording
│   ├── GetTransactionHistory()
│   └── Audit Trail Maintenance
└── Data Persistence
    ├── SaveData()
    └── LoadData()
```

### 3. **Persistence Abstraction Pattern**
`DataPersistence` abstracts storage details, allowing easy switching between storage mechanisms.

```
DataPersistence Interface:
├── SaveAccounts()      → Serialize to JSON, write to file
├── LoadAccounts()      → Read from file, deserialize JSON
├── DataFileExists()    → Check file existence
└── DeleteDataFile()    → Clear storage
```

### 4. **Model-View-Separation**
- **Models**: `Account`, `Transaction` (data structures)
- **Service**: `AccountService`, `IAccountService` (business logic)
- **Presentation**: `Program.cs` (user interface)

## Class Relationships

### Dependency Diagram
```
Program.cs
    │
    └─→ IAccountService (interface)
            │
            ├── CreateAccount()
            ├── Deposit()
            ├── Withdraw()
            └── ... other operations
            │
            │ implemented by
            │
            └─→ AccountService
                    │
                    ├─→ List<Account>
                    │       │
                    │       └─→ Account
                    │               │
                    │               └─→ List<Transaction>
                    │                       │
                    │                       └─→ Transaction
                    │
                    └─→ DataPersistence
                            │
                            └─→ JSON File Storage
```

### Class Composition
```
AccountService {
    - _accounts: List<Account>
    - _dataPersistence: DataPersistence
}

Account {
    - AccountNumber: string
    - AccountHolder: string
    - Balance: decimal
    - Transactions: List<Transaction>
    - CreatedDate: DateTime
    - LastModifiedDate: DateTime
    - IsActive: bool
}

Transaction {
    - TransactionId: string
    - Type: TransactionType (enum)
    - Amount: decimal
    - BalanceAfter: decimal
    - Timestamp: DateTime
    - Description: string
}

DataPersistence {
    - _filePath: string
    - _jsonOptions: JsonSerializerOptions
}
```

## Data Flow Diagrams

### Deposit Operation Flow
```
User Input: Deposit($500 to ACCT-123)
    │
    ▼
AccountService.Deposit(accountNumber, amount)
    │
    ├─ Validate amount > 0
    ├─ Find account by number
    ├─ Check if account is active
    │
    ▼
Account.Balance += amount
Account.LastModifiedDate = Now
    │
    ▼
Create Transaction record
    ├─ Type: Deposit
    ├─ Amount: $500
    ├─ BalanceAfter: new balance
    ├─ Timestamp: Now
    └─ Description: "Deposit of $500.00"
    │
    ▼
Account.Transactions.Add(transaction)
    │
    ▼
SaveData()
    ├─ Serialize accounts to JSON
    └─ Write to BankData/accounts.json
    │
    ▼
Return updated Account
```

### Load Operation Flow
```
Application Startup
    │
    ▼
new AccountService()
    │
    ├─ Create _dataPersistence
    ├─ Initialize _accounts = new List<Account>()
    │
    └─ LoadData()
        │
        ▼
        DataPersistence.LoadAccounts()
            │
            ├─ Check if BankData/accounts.json exists
            │
            ├─ YES: Read and deserialize JSON
            │       │
            │       └─ Parse to List<Account>
            │
            └─ NO: Return empty List<Account>
                │
                ▼
                _accounts populated with previous state
```

## Error Handling Strategy

### Validation Layers
```
User Operation (e.g., Withdraw)
    │
    ▼ Layer 1: Input Validation
Check amount > 0 ──────────────────┐
                                   │
    ▼ Layer 2: Account Validation  │
Check account exists               │
Check account is active            ├─→ ArgumentException
                                   │   InvalidOperationException
    ▼ Layer 3: Business Logic      │
Check sufficient funds             │
Update balance                      │
Record transaction                  │
                                   │
    ▼ Layer 4: Persistence         │
Save to file ─────────────────────┘
```

### Exception Types
```
ArgumentException
├─ Negative/zero amounts
├─ Null/empty strings
└─ Insufficient funds

InvalidOperationException
├─ Account not found
├─ Account is inactive
└─ Invalid operations on deleted accounts
```

## Collection Strategy

### Account Collection
```
_accounts: List<Account>

Operations:
├─ Add: O(1)              CreateAccount()
├─ Find: O(n)             GetAccount()
├─ Remove: O(n) logical   DeleteAccount() (mark inactive)
└─ Enumerate: O(n)        GetAllAccounts()

Indexing:
Account lookup uses FirstOrDefault(a => a.AccountNumber == number)
This is O(n) but acceptable for typical account counts
Could optimize with Dictionary<string, Account> for O(1) lookup
```

### Transaction Collection (per Account)
```
account.Transactions: List<Transaction>

Operations:
├─ Add: O(1)              Deposit(), Withdraw()
└─ Enumerate: O(n)        GetTransactionHistory()

Design Benefits:
├─ Immutable transactions (append-only)
├─ Complete audit trail
└─ No modification/deletion of records
```

## Persistence Strategy

### File Format: JSON
```json
[
  {
    "accountNumber": "...",
    "accountHolder": "...",
    "balance": 1000.00,
    ...
    "transactions": [
      { "transactionId": "...", ... },
      { "transactionId": "...", ... }
    ]
  }
]
```

### Advantages:
- ✓ Human-readable
- ✓ Platform-independent
- ✓ No external database required
- ✓ Easy to backup/restore
- ✓ Built-in .NET JSON support

### Data Durability:
```
Save on every operation:
├─ CreateAccount()
├─ Deposit()
├─ Withdraw()
├─ DeleteAccount()
└─ Manual SaveData()

Result: Accounts persisted to file automatically
```

## Testing Strategy

### Test Pyramid
```
        /\
       /  \     Unit Tests (35+)
      /    \    - Isolated method testing
     /  UT  \   - Mocks not needed
    /________\
    
    /  \          Integration Tests
   / IT  \        - Operation combinations
  /______\
  
  /  \              E2E Tests
 / E2E \            - Full workflows
/_______\
```

### Test Coverage Areas
```
CreateAccount()
├─ Valid creation
├─ Null/empty validation
├─ Unique account numbers
└─ Persistence

Deposit()
├─ Valid deposits
├─ Zero/negative validation
├─ Non-existent accounts
└─ Multiple deposits

Withdraw()
├─ Valid withdrawals
├─ Insufficient funds
├─ Amount validation
└─ Complete balance withdrawal

Retrieval
├─ Get single account
├─ Get all accounts
├─ Get balance
└─ Get transactions

Persistence
├─ Save operations
├─ Load operations
└─ Data integrity

Utilities
├─ Account counting
├─ Balance calculations
└─ Inactive handling
```

## Scalability Considerations

### Current Design
```
Suitable for:
├─ Single-user applications
├─ In-memory operations
├─ <100,000 accounts
└─ Local file storage
```

### Future Enhancements
```
For larger scale:
1. Replace List<Account> with Dictionary<string, Account>
   → O(1) account lookup instead of O(n)

2. Implement database persistence
   → Replace DataPersistence with DbContext
   → Use SQL for complex queries

3. Add async/await
   → Async file I/O
   → Non-blocking operations

4. Implement caching
   → Redis for frequently accessed accounts
   → Reduce file I/O

5. Multi-user support
   → Concurrency control
   → Transaction isolation
   → Locking mechanisms
```

## Security Considerations

### Current Implementation
```
Implemented:
✓ Input validation
✓ Amount validation
✓ Account existence checks
✓ Account status checks
```

### Not Implemented (for production)
```
To add for production:
├─ Authentication (who is the user?)
├─ Authorization (what can user do?)
├─ Encryption (encrypt sensitive data at rest)
├─ Audit logging (detailed operation logs)
├─ Rate limiting (prevent abuse)
└─ Concurrency control (prevent race conditions)
```

## Performance Analysis

### Time Complexity
```
Operation              | Time      | Space
CreateAccount()        | O(1)      | O(1)
Deposit()             | O(n)      | O(1)
Withdraw()            | O(n)      | O(1)
GetAccount()          | O(n)      | O(1)
GetAllAccounts()      | O(n)      | O(n)
GetBalance()          | O(n)      | O(1)
DeleteAccount()       | O(n)      | O(1)
SaveData()            | O(n)      | O(n)
LoadData()            | O(n)      | O(n)

n = number of accounts

Bottleneck: FindFirst() in GetAccount() → could use Dictionary for O(1)
```

### Memory Usage
```
Per Account:
├─ AccountNumber: ~20 bytes
├─ AccountHolder: ~50 bytes (average)
├─ Balance: 16 bytes (decimal)
├─ Dates: 32 bytes (2 x DateTime)
├─ IsActive: 1 byte (bool)
└─ Transactions: ~100 bytes per transaction (average)
   └─ Typical: 5-10 transactions

Total per account: ~300-400 bytes (without transactions)

Storage: JSON file grows linearly with accounts
```

## Conclusion

This architecture provides:
- ✓ Clean separation of concerns
- ✓ Testable design (interface-based)
- ✓ Extensible structure (easy to enhance)
- ✓ Maintainable codebase (well-organized)
- ✓ Reliable persistence (automatic saves)
- ✓ Comprehensive validation (prevent errors)

The design successfully demonstrates ADT principles and collection management while maintaining professional code quality standards.
