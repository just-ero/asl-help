using System;

namespace AslHelp.MemUtils.Exceptions;

internal class FatalNotFoundException : Exception
{
    public FatalNotFoundException()
        : base("The specified object could not be found.") { }

    public FatalNotFoundException(string message)
        : base(message) { }
}
