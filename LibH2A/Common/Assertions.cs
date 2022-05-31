using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Saber3D
{

  /// <summary>
  ///   A class that defines assertion macros.
  /// </summary>
  public static class Assertions
  {

    #region Assert Methods

    /// <summary>
    ///   Asserts that an expression is true.
    /// </summary>
    /// <param name="expression">
    ///   The expression to evaluate.
    /// </param>
    [DebuggerHidden]
    public static void Assert( bool expression )
    {
      if ( !expression )
        ThrowAssertionFailedException();
    }

    /// <summary>
    ///   Asserts that an expression is true.
    /// </summary>
    /// <param name="expression">
    ///   The expression to evaluate.
    /// </param>
    /// <param name="reason">
    ///   The reason the expression must be true.
    /// </param>
    [DebuggerHidden]
    public static void Assert( bool expression, string reason )
    {
      if ( !expression )
        ThrowAssertionFailedException( reason );
    }

    #endregion

    #region Fail Methods

    /// <summary>
    ///   Immediately causes a failed assertion.
    /// </summary>
    [DebuggerHidden]
    public static void Fail()
      => ThrowAssertionFailedException();

    /// <summary>
    ///   Immediately causes a failed assertion.
    /// </summary>
    /// <param name="reason">
    ///   The reason for the failure.
    /// </param>
    [DebuggerHidden]
    public static void Fail( string reason )
      => ThrowAssertionFailedException( reason );

    /// <summary>
    ///   Immediately causes a failed assertion and returns the default
    ///   value of the specified generic data type.
    /// </summary>
    /// <typeparam name="T">
    ///   The generic data type to "return".
    /// </typeparam>
    /// <returns>
    ///   The default value of the generic data type.
    /// </returns>
    /// <remarks>
    ///   This is a convenience method for when you want to immediately punt
    ///   from a call that is designed to return a value.
    /// </remarks>
    [DebuggerHidden]
    public static T FailReturn<T>()
    {
      ThrowAssertionFailedException();
      return default( T );
    }

    /// <summary>
    ///   Immediately causes a failed assertion and returns the default
    ///   value of the specified generic data type.
    /// </summary>
    /// <typeparam name="T">
    ///   The generic data type to "return".
    /// </typeparam>
    /// <param name="reason">
    ///   The reasoin for the failure.
    /// </param>
    /// <returns>
    ///   The default value of the generic data type.
    /// </returns>
    /// <remarks>
    ///   This is a convenience method for when you want to immediately punt
    ///   from a call that is designed to return a value.
    /// </remarks>
    [DebuggerHidden]
    public static T FailReturn<T>( string reason )
    {
      ThrowAssertionFailedException( reason );
      return default( T );
    }

    #endregion

    #region Helper Methods

    /// <summary>
    ///   Throws an empty <see cref="AssertionFailedException" />.
    /// </summary>
    [DebuggerHidden]
    [MethodImpl( MethodImplOptions.NoInlining )]
    private static void ThrowAssertionFailedException()
    {
      throw new AssertionFailedException();
    }

    /// <summary>
    ///   Throws an <see cref="AssertionFailedException" /> with a specified reason.
    /// </summary>
    /// <param name="reason">
    ///   The reason why the assertion failed.
    /// </param>
    [DebuggerHidden]
    [MethodImpl( MethodImplOptions.NoInlining )]
    private static void ThrowAssertionFailedException( string reason )
    {
      throw new AssertionFailedException( reason );
    }

    #endregion

  }

  /// <summary>
  ///   An <see cref="Exception" /> thrown when an assertion fails.
  /// </summary>
  public class AssertionFailedException : Exception
  {

    #region Constants

    /// <summary>
    ///   A generic, default exception message.
    /// </summary>
    private const string GENERIC_MESSAGE = "Assertion Failed.";

    #endregion

    #region Constructor

    /// <summary>
    ///   Constructs a new <see cref="AssertionFailedException" /> with a generic message.
    /// </summary>
    public AssertionFailedException()
      : base( GENERIC_MESSAGE )
    {
    }

    /// <summary>
    ///   Constructs a new <see cref="AssertionFailedException" /> with a specified message.
    /// </summary>
    /// <param name="message">
    ///   The assertion message.
    /// </param>
    public AssertionFailedException( string message )
      : base( BuildAssertionFailedMessage( message ) )
    {
    }

    #endregion

    #region Private Methods

    private static string BuildAssertionFailedMessage( string message )
    {
      if ( string.IsNullOrWhiteSpace( message ) )
        return GENERIC_MESSAGE;

      return $"Assertion Failed: {message}";
    }

    #endregion

  }

}
