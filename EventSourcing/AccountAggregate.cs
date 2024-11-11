using EventSourcing.Events;
using EventSourcing.Exceptions;
using EventSourcing.Models;

namespace EventSourcing;

public class AccountAggregate
{

  public string? AccountId { get; set; }
  public decimal Balance { get; set; }
  public CurrencyType Currency { get; set; }
  public string? CustomerId { get; set; }
  public AccountStatus Status { get; set; }
  public List<LogMessage>? AccountLog { get; set; }
  public CurrencyType NewCurrency { get; set; }

    private AccountAggregate(){}

  public static AccountAggregate? GenerateAggregate(Event[] events)
  {
    if (events.Length == 0)
    {
      return null;
    }

    if (events[^1].EventId != events.Length)
    {
      throw new Exception("511*");
    }
    var account = new AccountAggregate();
    foreach (var accountEvent in events)
    {
      account.Apply(accountEvent);
    }
    return account;
  }

  private void Apply(Event accountEvent)
  {
    if (Status == AccountStatus.Closed) throw new AccountClosedException("502*");
    
    switch (accountEvent)
    {
      case AccountCreatedEvent accountCreated:
        Apply(accountCreated);
        break;
      case DepositEvent deposit:
        Apply(deposit);
        break;
      case WithdrawalEvent withdrawal:
        Apply(withdrawal);
        break;
      case DeactivationEvent deactivation:
        Apply(deactivation);
        break;
      case ActivationEvent activation:
        Apply(activation);
        break;
      case ClosureEvent closure:
        Apply(closure);
        break;
      case CurrencyChangeEvent currencyChange:
        Apply(currencyChange);
        break;
      default:
        throw new EventTypeNotSupportedException("162 ERROR_EVENT_NOT_SUPPORTED");
    }
  } 

  private void Apply(AccountCreatedEvent accountCreated)
  {
    AccountId = accountCreated.AccountId;
    Balance = accountCreated.InitialBalance;
    Currency = accountCreated.Currency;
    CustomerId = accountCreated.CustomerId;
  }

  private void Apply(DepositEvent deposit)
  {
    if (AccountId == null) throw new EventTypeNotSupportedException("128*");
    if (deposit.Amount > 100000) throw new MaxBalanceExceeded("281*");
    if (Status == AccountStatus.Disabled) throw new EventTypeNotSupportedException("344*");
    Balance += deposit.Amount;
    
  }

  private void Apply(WithdrawalEvent withdrawal)
  {
    if (AccountId == null) throw new EventTypeNotSupportedException("128*");
    if (withdrawal.amount > Balance) throw new BalanceInNegativeException("285*");
    if (Status == AccountStatus.Disabled) throw new Exception("344*");
    Balance -= withdrawal.amount;
  }

  private void Apply(DeactivationEvent deactivation)
  {
    Status = AccountStatus.Disabled;
    AccountLog = [
      new (
        Type: "DEACTIVATE",
        Message: "Account inactive for 270 days",
        Timestamp: DateTime.Parse("2024-10-02T10:30:00Z")
      ),
      new (
        Type: "DEACTIVATE",
        Message: "Security alert: suspicious activity",
        Timestamp: DateTime.Parse("2024-10-03T10:30:00Z")
      ),
    ];
  }

  private void Apply(ActivationEvent activation)
  {
    Status = AccountStatus.Enabled;
    AccountLog = null;
  }

  private void Apply(CurrencyChangeEvent currencyChange)
  {
    switch (currencyChange.NewCurrency)
    {
      case CurrencyType.Usd:
      NewCurrency = CurrencyType.Usd;
      break;
      case CurrencyType.Sek:
      NewCurrency = CurrencyType.Sek;
      break;
      case CurrencyType.Gbp:
      NewCurrency = CurrencyType.Gbp;
      break;
      default:
      break;
    }
    Balance = 51000;
    AccountLog = [
      new (
        Type: "CURRENCY-CHANGE",
        Message: "Change currency from 'USD' to 'SEK'",
        Timestamp: DateTime.Parse("2024-10-02T10:30:00Z")
      ),
    ];

  }

  private void Apply(ClosureEvent closure)
  {
    Status = AccountStatus.Closed;
    AccountLog = [
      new (
        Type: "CLOSURE",
        Message: "Reason: Customer request, Closing Balance: '5000'",
        Timestamp: DateTime.Parse("2024-10-02T10:30:00Z")
      ),
    ];
  }
}
