using System.Collections;
using System.Collections.Generic;

namespace AslHelp.Shared.Logging;

/// <summary>
///     Represents a collection of <see cref="ILogger"/> instances that can be used together.
/// </summary>
public sealed class MultiLogger : ILogger, IList<ILogger>
{
    private readonly List<ILogger> _loggers;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MultiLogger"/> class
    ///     with the specified <see cref="ILogger"/>s.
    /// </summary>
    public MultiLogger(params ILogger[] loggers)
    {
        _loggers = [.. loggers];
    }

    /// <summary>
    ///     Gets the amount of loggers contained in the <see cref="MultiLogger"/>.
    /// </summary>
    public int Count => _loggers.Count;

    /// <summary>
    ///     Gets a value indicating whether the <see cref="MultiLogger"/> is read-only.
    /// </summary>
    public bool IsReadOnly => ((ICollection<ILogger>)_loggers).IsReadOnly;

    /// <summary>
    ///     Gets or sets the <see cref="ILogger"/> at the specified index.
    /// </summary>
    public ILogger this[int index]
    {
        get => _loggers[index];
        set => _loggers[index] = value;
    }

    /// <summary>
    ///     Starts all loggers contained in the <see cref="MultiLogger"/>.
    /// </summary>
    public void Start()
    {
        for (int i = 0; i < _loggers.Count; i++)
        {
            _loggers[i]?.Start();
        }
    }

    /// <summary>
    ///     Writes an empty line to all loggers contained in the <see cref="MultiLogger"/>.
    /// </summary>
    public void Log()
    {
        Log("");
    }

    /// <summary>
    ///     Writes the string representation of the specified value to all loggers contained in the <see cref="MultiLogger"/>.
    /// </summary>
    /// <param name="output">The value to log.</param>
    public void Log(object? output)
    {
        for (int i = 0; i < _loggers.Count; i++)
        {
            _loggers[i]?.Log(output);
        }
    }

    /// <summary>
    ///     Stops all contained loggers.
    /// </summary>
    public void Stop()
    {
        for (int i = 0; i < _loggers.Count; i++)
        {
            _loggers[i]?.Stop();
        }
    }

    /// <summary>
    ///     Adds an <see cref="ILogger"/> to the <see cref="MultiLogger"/>.
    /// </summary>
    /// <param name="item">The <see cref="ILogger"/> to add.</param>
    public void Add(ILogger item)
    {
        _loggers.Add(item);
    }

    /// <summary>
    ///     Inserts an <see cref="ILogger"/> into the <see cref="MultiLogger"/> at the specified index.
    /// </summary>
    /// <param name="index">The index at which to insert the logger.</param>
    /// <param name="item">The logger to insert.</param>
    public void Insert(int index, ILogger item)
    {
        _loggers.Insert(index, item);
    }

    /// <summary>
    ///     Removes the first occurrence of a specific <see cref="ILogger"/> from the <see cref="MultiLogger"/>.
    /// </summary>
    /// <param name="item">The logger to remove.</param>
    /// <returns>
    ///     <see langword="true"/> if <paramref name="item"/> was successfully removed from the <see cref="MultiLogger"/>;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    public bool Remove(ILogger item)
    {
        return _loggers.Remove(item);
    }

    /// <summary>
    ///     Removes the <see cref="ILogger"/> at the specified index of the <see cref="MultiLogger"/>.
    /// </summary>
    /// <param name="index">The index of the logger to remove.</param>
    public void RemoveAt(int index)
    {
        _loggers.RemoveAt(index);
    }

    /// <summary>
    ///     Removes all <see cref="ILogger"/> instances from the <see cref="MultiLogger"/>.
    /// </summary>
    public void Clear()
    {
        _loggers.Clear();
    }

    /// <summary>
    ///     Determines whether the <see cref="MultiLogger"/> contains a specified <see cref="ILogger"/>.
    /// </summary>
    /// <param name="item">The <see cref="ILogger"/> to locate in the <see cref="MultiLogger"/>.</param>
    /// <returns>
    ///     <see langword="true"/> if <paramref name="item"/> is found in the <see cref="MultiLogger"/>;
    ///     otherwise, <see langword="false"/>.
    /// </returns>
    public bool Contains(ILogger item)
    {
        return _loggers.Contains(item);
    }

    /// <summary>
    ///     Determines the index of a specified <see cref="ILogger"/> in the <see cref="MultiLogger"/>.
    /// </summary>
    /// <param name="item">The <see cref="ILogger"/> to locate in the <see cref="MultiLogger"/>.</param>
    /// <returns>
    ///     The index of <paramref name="item"/> if found in the <see cref="MultiLogger"/>;
    ///     otherwise, <c>-1</c>.
    /// </returns>
    public int IndexOf(ILogger item)
    {
        return _loggers.IndexOf(item);
    }

    /// <summary>
    ///     Copies the elements of the <see cref="MultiLogger"/> to an <see cref="Array"/>,
    ///     starting at a particular <see cref="Array"/> index.
    /// </summary>
    /// <param name="array">
    ///     The <see cref="Array"/> that is the destination of the elements copied from the <see cref="MultiLogger"/>.
    /// </param>
    /// <param name="arrayIndex">The index in <paramref name="array"/> at which copying begins.</param>
    public void CopyTo(ILogger[] array, int arrayIndex)
    {
        _loggers.CopyTo(array, arrayIndex);
    }

    /// <summary>
    ///     Returns an enumerator that iterates through the <see cref="MultiLogger"/>.
    /// </summary>
    public IEnumerator<ILogger> GetEnumerator()
    {
        return _loggers.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _loggers.GetEnumerator();
    }
}
