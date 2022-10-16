using System;

namespace AslHelp.MemUtils.Exceptions;

internal class InvalidAddressException : Exception
{
    public InvalidAddressException()
        : base("Address is invalid.") { }

    public InvalidAddressException(string message)
        : base(message) { }
}
