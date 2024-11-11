namespace EventSourcing.Exceptions;

public class BalanceInNegativeException(string message) : InvalidOperationException(message);