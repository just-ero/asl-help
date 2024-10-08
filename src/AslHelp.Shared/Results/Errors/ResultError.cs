namespace AslHelp.Shared.Results.Errors;

public abstract record ResultError(
    string Message) : IResultError
{
    public sealed override string ToString()
    {
        if (this is ExceptionError ex)
        {
            return $"{ex.Exception.GetType().Name}: {Message}";
        }
        else
        {
            return $"{GetType().Name}: {Message}";
        }
    }
}
