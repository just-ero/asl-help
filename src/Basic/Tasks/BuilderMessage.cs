namespace AslHelp.Tasks;

public class BuilderMessage<TResult>
{
    private enum MessageType
    {
        Plain,
        Func
    }

    private readonly MessageType _type;

    private readonly string _message;
    private readonly Func<TResult, string> _messageFunc;

    public BuilderMessage(string message)
    {
        _type = MessageType.Plain;
        _message = message;
    }

    public BuilderMessage(Func<TResult, string> messageFunc)
    {
        _type = MessageType.Func;
        _messageFunc = messageFunc;
    }

    public void Send(TResult result = default)
    {
        Debug.Info(_type switch
        {
            MessageType.Plain => _message,
            MessageType.Func => _messageFunc(result),
            _ => throw new InvalidOperationException()
        });
    }
}
