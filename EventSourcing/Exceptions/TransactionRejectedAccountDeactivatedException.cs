namespace EventSourcing.Exceptions;

public class TransactionRejectedAccountDeactivatedException(string message) : InvalidOperationException(message);