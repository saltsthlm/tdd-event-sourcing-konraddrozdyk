namespace EventSourcing.Exceptions;

public class InvalidEventStreamException(string message) : InvalidOperationException(message);
