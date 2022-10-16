using System;

namespace AslHelp.MemUtils.Exceptions;

internal class OperationFailedException : Exception
{
    public OperationFailedException()
        : base("The operation failed to execute.") { }

    public OperationFailedException(string message)
        : base(message) { }
}
