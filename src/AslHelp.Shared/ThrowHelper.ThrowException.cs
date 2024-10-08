using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace AslHelp.Shared;

public static partial class ThrowHelper
{
    /// <summary>
    ///     Throws an <see cref="ArgumentException"/> with a specified error message
    ///     and the name of the parameter that causes this exception.
    /// </summary>
    /// <param name="paramName">The name of the parameter that caused the exception.</param>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <remarks>
    ///     This method does not return and is not inlined to improve codegen of cold paths.
    /// </remarks>
    /// <exception cref="ArgumentException"/>
    [DoesNotReturn]
    public static void ThrowArgumentException(string paramName, string message)
    {
        throw new ArgumentException(message, paramName);
    }

    /// <summary>
    ///     Throws an <see cref="ArgumentNullException"/> with a specified error message
    ///     and the name of the parameter that causes this exception.
    /// </summary>
    /// <param name="paramName">The name of the parameter that caused the exception.</param>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <remarks>
    ///     This method does not return and is not inlined to improve codegen of cold paths.
    /// </remarks>
    /// <exception cref="ArgumentNullException"/>
    [DoesNotReturn]
    public static void ThrowArgumentNullException(string paramName, string message)
    {
        throw new ArgumentNullException(paramName, message);
    }

    /// <summary>
    ///     Throws an <see cref="ArgumentOutOfRangeException"/> with a specified error message
    ///     and the name of the parameter that causes this exception.
    /// </summary>
    /// <param name="paramName">The name of the parameter that caused the exception.</param>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <remarks>
    ///     This method does not return and is not inlined to improve codegen of cold paths.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException"/>
    [DoesNotReturn]
    public static void ThrowArgumentOutOfRangeException(string paramName, string message)
    {
        throw new ArgumentOutOfRangeException(message, paramName);
    }

    /// <summary>
    ///     Throws a <see cref="DirectoryNotFoundException"/> with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <remarks>
    ///     This method does not return and is not inlined to improve codegen of cold paths.
    /// </remarks>
    /// <exception cref="DirectoryNotFoundException"/>
    [DoesNotReturn]
    public static void ThrowDirectoryNotFoundException(string message)
    {
        throw new DirectoryNotFoundException(message);
    }

    /// <summary>
    ///     Throws an <see cref="Exception"/> with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <remarks>
    ///     This method does not return and is not inlined to improve codegen of cold paths.
    /// </remarks>
    /// <exception cref="Exception"/>
    [DoesNotReturn]
    public static void ThrowException(string message)
    {
        throw new Exception(message);
    }

    /// <summary>
    ///     Throws a <see cref="FileNotFoundException"/> with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <remarks>
    ///     This method does not return and is not inlined to improve codegen of cold paths.
    /// </remarks>
    /// <exception cref="FileNotFoundException"/>
    [DoesNotReturn]
    public static void ThrowFileNotFoundException(string message)
    {
        throw new FileNotFoundException(message);
    }

    /// <summary>
    ///     Throws a <see cref="FormatException"/> with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <remarks>
    ///     This method does not return and is not inlined to improve codegen of cold paths.
    /// </remarks>
    /// <exception cref="FormatException"/>
    [DoesNotReturn]
    public static void ThrowFormatException(string message)
    {
        throw new FormatException(message);
    }

    /// <summary>
    ///     Throws an <see cref="InvalidDataException"/> with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <remarks>
    ///     This method does not return and is not inlined to improve codegen of cold paths.
    /// </remarks>
    /// <exception cref="InvalidDataException"/>
    [DoesNotReturn]
    public static void ThrowInvalidDataException(string message)
    {
        throw new InvalidDataException(message);
    }

    /// <summary>
    ///     Throws an <see cref="InvalidOperationException"/> with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <remarks>
    ///     This method does not return and is not inlined to improve codegen of cold paths.
    /// </remarks>
    /// <exception cref="InvalidOperationException"/>
    [DoesNotReturn]
    public static void ThrowInvalidOperationException(string message)
    {
        throw new InvalidOperationException(message);
    }

    /// <summary>
    ///     Throws a <see cref="KeyNotFoundException"/> with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <remarks>
    ///     This method does not return and is not inlined to improve codegen of cold paths.
    /// </remarks>
    /// <exception cref="KeyNotFoundException"/>
    [DoesNotReturn]
    public static void ThrowKeyNotFoundException(string message)
    {
        throw new KeyNotFoundException(message);
    }

    /// <summary>
    ///     Throws a <see cref="NotImplementedException"/> with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <remarks>
    ///     This method does not return and is not inlined to improve codegen of cold paths.
    /// </remarks>
    /// <exception cref="NotImplementedException"/>
    [DoesNotReturn]
    public static void ThrowNotImplementedException(string message)
    {
        throw new NotImplementedException(message);
    }

    /// <summary>
    ///     Throws a <see cref="NotSupportedException"/> with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <remarks>
    ///     This method does not return and is not inlined to improve codegen of cold paths.
    /// </remarks>
    /// <exception cref="NotSupportedException"/>
    [DoesNotReturn]
    public static void ThrowNotSupportedException(string message)
    {
        throw new NotSupportedException(message);
    }

    /// <summary>
    ///     Throws an <see cref="ObjectDisposedException"/> for a specified object.
    /// </summary>
    /// <param name="objectName">The name of the disposed object.</param>
    /// <remarks>
    ///     This method does not return and is not inlined to improve codegen of cold paths.
    /// </remarks>
    /// <exception cref="ObjectDisposedException"/>
    [DoesNotReturn]
    public static void ThrowObjectDisposedException(string objectName)
    {
        throw new ObjectDisposedException(objectName);
    }

    /// <summary>
    ///     Throws a <see cref="Win32Exception"/>.
    /// </summary>
    /// <remarks>
    ///     This method does not return and is not inlined to improve codegen of cold paths.
    /// </remarks>
    /// <exception cref="Win32Exception"/>
    [DoesNotReturn]
    public static void ThrowWin32Exception()
    {
        throw new Win32Exception();
    }
}
