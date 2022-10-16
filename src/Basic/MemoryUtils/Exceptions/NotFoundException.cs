using System;

namespace AslHelp.MemUtils.Exceptions;

internal class NotFoundException : Exception
{
    public NotFoundException()
        : base("The specified object could not be found.") { }

    public NotFoundException(string message)
        : base(message) { }
}
