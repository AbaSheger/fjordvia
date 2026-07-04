namespace Fjordvia.Core.Exceptions;

public sealed class DomainValidationException(string message) : Exception(message);
