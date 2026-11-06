# C# Learn Bank

Учебная модель банковской системы. Предметная область учебного проекта подталкивает к плотному и правильному использованию ООП.
Существуют разные типы счетов с разным поведением, разные виды транзакций, правила, отчеты и так далее.
В такой задаче наследование, абстракции, полиморфизм, паттерны и многое другое используется естественно.

## Статус

Проект в начале разработки: создан скелет репозитория и зафиксирована архитектура, наполнение доменной модели законченно. API и структура слоёв ещё будут меняться.

## Стек и требования

- .NET SDK 10.0
- C#
- Форматирование: [CSharpier](https://csharpier.com/) (конфигурация в `.csharpierrc.json`)
- Тесты: фреймворк будет выбран при добавлении `Tests/`

## Архитектура

```mermaid
flowchart TD
    U[UI] --> A
    A[App] --> D
    A[App] --> I
    I[Infrastructure] --> D
    D[Domain]
```

- **UI** — консольное меню и вывод. Вызывает только `BankService`.
- **App** — сценарии: переводы, проценты, отчёты (`BankService`).
- **Infrastructure** — логирование, время, генерация id и так далее.
- **Domain** — сущности и правила: `Account`, `Customer`, `Transaction` и другие.

## Поток выполнения

Единственная точка входа — `Program.Main` (composition root).

```text
Запуск процесса
  └─> Program.Main()
        ├─ создать ConsoleLogger (реализация ILogger)
        ├─ try:
        │    └─> Compose(logger)              // сборка графа зависимостей
        │          ├─ new SystemClock()                    : IClock
        │          ├─ new InMemoryRepository<Account>()     : IRepository<Account>
        │          ├─ new InMemoryRepository<Customer>()    : IRepository<Customer>
        │          ├─ new InMemoryRepository<Transaction>() : IRepository<Transaction>
        │          ├─ new TransactionRecorder(transactions, clock, logger)
        │          ├─ new BankService(customers, accounts, transfers, interest, reports)
        │          └─ new ConsoleMenu(service, logger)
        │    └─> menu.Run()                   // запуск UI
        │          └─> BankService.<сценарии>
        └─ catch (Exception ex):
              └─> logger.Error(...)           // централизованная обработка
Завершение процесса
```

Направление зависимостей строго внутрь: `UI → App → Domain`, `Infrastructure → Domain`.

### Composition root

Все зависимости внедряются через конструкторы.

```csharp
private static ConsoleMenu Compose(ILogger logger)
{
    IClock clock = new SystemClock();
    IRepository<Account> accounts = new InMemoryRepository<Account>();
    IRepository<Customer> customers = new InMemoryRepository<Customer>();
    IRepository<Transaction> transactions = new InMemoryRepository<Transaction>();

    var recorder = new TransactionRecorder(transactions, clock, logger);

    var service = new BankService(
        new CustomerService(customers),
        new AccountService(accounts, customers, recorder),
        new TransferService(accounts, recorder),
        new InterestService(accounts, recorder),
        new ReportService(accounts, customers, transactions)
    );
    return new ConsoleMenu(service, logger);
}
```

`Compose()` — единственное место, где конкретные классы инфраструктуры связываются с портами:

```mermaid
flowchart TD
    P["Program / Compose()<br/>(composition root)"]

    P -->|"new"| SC["SystemClock"]
    P -->|"new"| AR["InMemoryRepository&lt;Account&gt;"]
    P -->|"new"| CR["InMemoryRepository&lt;Customer&gt;"]
    P -->|"new"| TR["InMemoryRepository&lt;Transaction&gt;"]
    P -->|"new"| CL["ConsoleLogger"]

    SC -.->|"IClock"| BS["BankService"]
    AR -.->|"IRepository"| BS
    CR -.->|"IRepository"| BS
    TR -.->|"IRepository"| BS
    CL -.->|"ILogger"| BS

    BS -->|"сценарии"| UI["ConsoleMenu"]
    CL -.->|"ILogger"| UI
```

### Прикладной цикл

```text
меню → выбор команды → BankService.<под-фасад> → доменные сущности + репозиторий → результат в UI
```

Ошибки домена ловятся на уровне UI/App и показываются пользователю.

## Доменная модель

```mermaid
classDiagram
    direction TB

    class Account {
        <<abstract>>
        +Guid Id
        +string Number
        +decimal Balance
        +Deposit(amount) void
        +Withdraw(amount)* void
    }

    class DebitAccount {
        +Withdraw(amount) void
    }

    class CreditAccount {
        -decimal CreditLimit
        +Deposit(amount) void
        +Withdraw(amount) void
    }

    class Customer {
        +Guid Id
        +string Name
        +IReadOnlyList~Account~ Accounts
        +TotalBalance() decimal
    }

    class Transaction {
        +Guid Id
        +DateTime Date
        +TransactionType Type
        +decimal Amount
        +Guid? FromAccountId
        +Guid? ToAccountId
    }

    class TransactionType {
        <<enumeration>>
        Deposit
        Withdraw
        Transfer
        Interest
    }

    class IRepository~T~ {
        <<interface>>
        +GetById(id) T
        +Add(entity) void
    }

    DebitAccount --|> Account : extends
    CreditAccount --|> Account : extends
    Customer "1" o-- "0..*" Account : owns
    Transaction --> TransactionType : type
    IRepository~T~ ..> Transaction : история по AccountId
```

```text
Account (абстракция)
├── CreditAccount   кредитный счёт 
└── DebitAccount    дебетовый счёт
Transaction         запись операций
Customer            клиент
```

## Интерфейсы и расширяемость

- `IAccount` — единый контракт счёта.
- `IInterestBearing` — «умеет начислять проценты».
- `IRepository<T>` — абстракция хранения (JSON, память, БД, фейк в тестах).
- `ILogger`, `IClock` — точки расширения и тестируемости, чтобы не звать `DateTime` и `Console` напрямую.
- Событие `TransactionCompleted` — на него подписываются логирование и аудит; сервис не знает, кто реагирует.

### Infrastructure

Конкретные реализации портов. Каждая живёт в своей папке (`Persistence/`, `Time/`, `Logging/`, `Identity/`).

```mermaid
classDiagram
    direction LR

    class IRepository~T~ {
        <<interface>>
        +GetById(id) T
        +GetAll() IReadOnlyCollection~T~
        +Add(entity) void
    }
    class IClock {
        <<interface>>
        +DateTime Now
    }
    class ILogger {
        <<interface>>
        +Info(message) void
        +Warn(message) void
        +Error(message, exception) void
    }

    class InMemoryRepository~T~ {
        -Dictionary~Guid,T~ _items
        +GetById(id) T
        +GetAll() IReadOnlyCollection~T~
        +Add(entity) void
    }
    class SystemClock {
        +DateTime Now
    }
    class ConsoleLogger {
        +Info(message) void
        +Warn(message) void
        +Error(message, exception) void
    }

    InMemoryRepository~T~ ..|> IRepository~T~ : implements
    SystemClock ..|> IClock : implements
    ConsoleLogger ..|> ILogger : implements
```

### Service (Application)

`BankService` — фасад прикладного слоя. Он не содержит бизнес-логики: агрегирует
под-фасады, каждый из которых отвечает за свой сценарий (деление по use case).

```text
BankService
├── Customers : CustomerService   регистрация и поиск клиентов
├── Accounts  : AccountService    открытие счетов, пополнение, снятие
├── Transfers : TransferService   переводы между счетами
├── Interest  : InterestService   начисление процентов
└── Reports   : ReportService     выписки и итоговые балансы
```

Под-фасады знают только порты (`IRepository<T>`, `IClock`, `ILogger`), а не их реализации.
Сквозная логика — запись `Transaction`, событие `TransactionCompleted` и логирование —
вынесена в общий коллаборатор `TransactionRecorder`, чтобы не дублировать её в каждом под-фасаде:

```mermaid
classDiagram
    direction TB

    class BankService {
        +CustomerService Customers
        +AccountService Accounts
        +TransferService Transfers
        +InterestService Interest
        +ReportService Reports
    }

    class CustomerService
    class AccountService
    class TransferService
    class InterestService
    class ReportService

    class TransactionRecorder {
        +Record(type, amount, from, to) Transaction
        +event TransactionCompleted
    }

    class IRepository~T~ {
        <<interface>>
    }
    class IClock {
        <<interface>>
    }
    class ILogger {
        <<interface>>
    }

    BankService --> CustomerService
    BankService --> AccountService
    BankService --> TransferService
    BankService --> InterestService
    BankService --> ReportService

    CustomerService --> IRepository~T~ : customers
    AccountService --> IRepository~T~ : accounts
    TransferService --> IRepository~T~ : accounts
    InterestService --> IRepository~T~ : accounts
    ReportService --> IRepository~T~ : accounts, customers, transactions

    AccountService --> TransactionRecorder
    TransferService --> TransactionRecorder
    InterestService --> TransactionRecorder

    TransactionRecorder --> IClock
    TransactionRecorder --> ILogger
```

UI зависит только от `BankService`; конкретику подставляет composition root.

## Структура проекта

```text
C#-learn-bank/
├── src/
│   ├── Domain/             Account, Customer, Transaction, интерфейсы домена
│   ├── Application/        BankService-агрегатор, под-фасады (Customers, Accounts, Transfers, Interest, Reports)
│   ├── Infrastructure/     Persistence/, Logging/, Time/
│   └── UI/                 консольное меню
├── Tests/                  юнит-тесты (будет добавлен)
├── Program.cs              composition root, точка входа
├── C#-learn-bank.csproj
├── .csharpierrc.json
├── LICENCE
└── README.md
```

## Сборка и запуск

```bash
dotnet build
dotnet run
```

> Точка входа (`Program.Main` / composition root) реализуется в рамках инфраструктурного этапа
> вместе с `BankService` и консольным меню.

## Форматирование

```bash
dotnet tool install -g csharpier
csharpier format .
```

Настройки в `.csharpierrc.json`.

## Тестирование

Юнит-тесты — отдельный этап. Тестируемость заложена заранее: `IClock`, `ILogger` и `IRepository<T>`.
Запуск будет выглядеть как `dotnet test`.

## Лицензия

MIT. См. [LICENCE](LICENCE).
