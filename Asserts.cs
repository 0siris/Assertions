using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Assertions;

public static class Asserts {
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int AssertValue(
        this int input,
        int required,
        string message = "Boolean expression asserted to be true is false.",
        [CallerArgumentExpression("input")] string? expression = null
    ) =>
        input == required ? input : throw new AssertException(message, expression);

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AssertTrue(
        [DoesNotReturnIf(false)] this bool booleanExpression,
        string message = "Boolean expression asserted to be true is false.",
        [CallerArgumentExpression("booleanExpression")]
        string? expression = null
    ) =>
        booleanExpression ? true : throw new AssertException(message, expression);



    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AssertFalse<T>(
        [DoesNotReturnIf(true)] this bool booleanExpression,
        string message = "Boolean expression asserted to be true is false.",
        [CallerArgumentExpression("booleanExpression")]
        string? expression = null,
        ThrowException<T>? makeException = null
    ) where T:Exception {
        if (!booleanExpression)
            return false;

        if (makeException?.Invoke(message) is {} e)
            throw e;
        
        throw new AssertException(message, expression);
    }

    public delegate T ThrowException<out T>(string message) where T:Exception;

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AssertFalse(
        [DoesNotReturnIf(true)] this bool booleanExpression,
        string message = "Boolean expression asserted to be false is true.",
        [CallerArgumentExpression("booleanExpression")]
        string? expression = null
    ) =>
        booleanExpression ? throw new AssertException(message, expression) : false;

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T AssertNotNull<T>(
        [NotNull] this T? obj,
        string? message = null,
        [CallerArgumentExpression("obj")] string? expression = null
    ) where T : class =>
        obj ?? throw new AssertException(message ?? "Object is null.") {
            Expression = expression,
        };

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T AssertNotNull<T>(
        this T? obj,
        string? message = null,
        [CallerArgumentExpression("obj")] string? expression = null
    ) where T : struct {
        if (obj is { } o)
            return o;

        throw new AssertException(message ?? "Object is null.") {
            Expression = expression,
        };
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T AssertArgumentNotNull<T>(
        [NotNull] this T? obj,
        string? message = null,
        [CallerArgumentExpression("obj")] string? argName = null
    ) where T : class =>
        obj ?? throw new ArgumentNullException(argName, message);

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T AssertArgumentNotNull<T>(
        this T? obj,
        string? message = null,
        [CallerArgumentExpression("obj")] string? argName = null
    ) where T : struct {
        if (obj is { } value)
            return value;

        throw new ArgumentNullException(argName, message);
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AssertNull<T>(
        this T? obj,
        string? message = null,
        [CallerArgumentExpression("obj")] string? expression = null
    ) where T : class {
        if (obj is not null)
            throw new AssertException(message ?? "Object must be null.") {
                Expression = expression,
            };
    }

    /// <summary>
    /// Checks if the value is in the given range
    /// </summary>
    /// <typeparam name="T">the number type</typeparam>
    /// <param name="value">the number</param>
    /// <param name="lower">the lower bound (inclusive) </param>
    /// <param name="upper">the upper bound (Inclusive) </param>
    /// <param name="argName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T AssertArgumentRange<T>(
        this T value,
        T lower,
        T upper,
        [CallerArgumentExpression("value")] string? argName = null
    ) where T : INumber<T> =>
        lower <= value && value <= upper
            ? value
            : throw new ArgumentOutOfRangeException(argName, value, $"{argName} is out of range!");

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NewType AssertTypeOf<NewType>(this object? obj) {
        if (obj is NewType c)
            return c;
        throw new AssertException<object>($"Obj is not type of {typeof(NewType).Name}", obj);
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T AssertLessOrEqual<T>(this T value, T upperLimit) where T : INumber<T> {
        (value <= upperLimit).AssertTrue();
        return value;
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T AssertLess<T>(this T value, T upperLimit) where T : INumber<T> {
        (value < upperLimit).AssertTrue();
        return value;
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static char AssertLetter(this char letter, string? errorMessage = null) {
        if (!char.IsLetter(letter))
            throw new AssertException(errorMessage ?? "char must be alphabetic.");
        return letter;
    }

    /// <summary>
    /// Asserts that the character is alphabetic and returns it unchanged.
    /// </summary>
    /// <param name="value">Character to validate.</param>
    /// <param name="errorMessage">Optional custom assertion message.</param>
    /// <returns>The validated alphabetic character.</returns>
    /// <exception cref="AssertException">Thrown when <paramref name="value" /> is not alphabetic.</exception>
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static char AssertIsLetter(this char value, string? errorMessage = null) {
        if (!char.IsLetter(value))
            throw new AssertException(errorMessage ?? "char must be alphabetic.");

        return value;
    }
}

public class AssertException : Exception {
    public string? Expression { get; init; }
    
    public AssertException(string? message) : base(message) { }

    public AssertException(string? message, string? expression) : base(message is not null && expression is not null
                                                                           ? message + "Expression: " + expression
                                                                           : message ?? expression) { }

    public AssertException(string? message, Exception? innerException) : base(message,
                                                                              innerException) { }
}

public class AssertException<T> : AssertException {
    public readonly T Value;

    public AssertException(string? message, T value) : base(message) => Value = value;

    public AssertException(string? message, T value, Exception? innerException) : base(message, innerException) =>
        Value = value;
}
