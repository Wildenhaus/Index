using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static Saber3D.Assertions;

namespace Saber3D.Serializers.Scripting.Text
{

  public class ScriptingTextReader : ScriptingReader
  {

    #region Constants

    private const int BUFFER_SIZE = 4096;

    #endregion

    #region Data Members

    private static readonly string[] TOKEN_SPLIT_DELIMITERS = new string[] { " ", "," };

    private readonly Stream _baseStream;
    private readonly TextReader _reader;

    private Queue<string> _tokenBuffer;

    #endregion

    #region Properties

    public bool EndOfStream
    {
      get;
      private set;
    }

    #endregion

    #region Constructor

    public ScriptingTextReader( TextReader reader )
    {
      Assert( reader != null, "Reader cannot be null." );

      _reader = reader;
      _tokenBuffer = new Queue<string>();
    }

    public ScriptingTextReader( Stream stream, bool keepStreamOpen = true )
    {
      Assert( stream != null, "Stream cannot be null." );

      _baseStream = stream;
      _reader = new StreamReader( stream, Encoding.UTF8, false, BUFFER_SIZE, keepStreamOpen );
      _tokenBuffer = new Queue<string>();
    }

    #endregion

    #region Public Methods

    public override bool Read()
    {
      if ( _tokenBuffer.Count == 0 && !TryBufferTokens() )
      {
        EndOfStream = true;
        return false;
      }

      Token = _tokenBuffer.Dequeue();
      TokenType = ParseTokenType( Token );
      return true;
    }

    #endregion

    #region Private Methods

    private bool TryBufferTokens()
    {
      while ( true )
      {
        var line = _reader.ReadLine()?.Trim();

        // Check for EOF
        if ( line is null )
          return false;

        // Skip empty lines
        if ( string.IsNullOrWhiteSpace( line ) )
          continue;

        // Skip comment lines
        if ( line.StartsWith( "//" ) )
          continue;

        foreach ( var token in line.Split( TOKEN_SPLIT_DELIMITERS, StringSplitOptions.RemoveEmptyEntries ) )
          _tokenBuffer.Enqueue( token );

        return true;
      }
    }

    private ScriptingTokenType ParseTokenType( string token )
    {
      switch ( token )
      {
        case "=": return ScriptingTokenType.Assignment;
        case "{": return ScriptingTokenType.StartObject;
        case "}": return ScriptingTokenType.EndObject;
        case "[": return ScriptingTokenType.StartArray;
        case "]": return ScriptingTokenType.EndArray;
      }

      if ( TokenType == ScriptingTokenType.Assignment )
        return ScriptingTokenType.Value;

      return ScriptingTokenType.PropertyName;
    }

    #endregion

  }

}
