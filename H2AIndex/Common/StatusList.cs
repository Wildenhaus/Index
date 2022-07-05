using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static H2AIndex.Common.StatusList;

namespace H2AIndex.Common
{

  public class StatusList : IEnumerable<Entry>
  {

    #region Data Members

    private readonly List<Entry> _messages;
    private readonly List<Entry> _warnings;
    private readonly List<Entry> _errors;

    #endregion

    #region Properties

    public IReadOnlyList<Entry> Messages => _messages;
    public IReadOnlyList<Entry> Warnings => _warnings;
    public IReadOnlyList<Entry> Errors => _errors;

    public int Count => _messages.Count + _warnings.Count + _errors.Count;

    public bool HasMessages => _messages.Count > 0;
    public bool HasWarnings => _warnings.Count > 0;
    public bool HasErrors => _errors.Count > 0;

    #endregion

    #region Constructor

    public StatusList()
    {
      _messages = new List<Entry>();
      _warnings = new List<Entry>();
      _errors = new List<Entry>();
    }

    #endregion

    #region Public Methods

    public void AddMessage( string name, string message )
        => _messages.Add( new Entry( EntryType.Message, name, message ) );

    public void AddWarning( string name, string message )
        => _warnings.Add( new Entry( EntryType.Warning, name, message ) );

    public void AddError( string name, string message, Exception exception = null )
        => _errors.Add( new Entry( EntryType.Error, name, message, exception ) );

    public void AddError( string name, Exception exception )
      => _errors.Add( new Entry( EntryType.Error, name, exception.Message, exception ) );

    #endregion

    #region IEnumerable Methods

    public IEnumerator<Entry> GetEnumerator()
    {
      var entries = _messages.Concat( _warnings ).Concat( _errors );
      foreach ( var entry in entries )
        yield return entry;
    }

    IEnumerator IEnumerable.GetEnumerator()
      => GetEnumerator();

    #endregion

    #region Embedded Types

    public enum EntryType
    {
      Message = 1,
      Warning = 2,
      Error = 3
    }

    public struct Entry
    {

      #region Data Members

      public readonly EntryType Type;
      public readonly string Name;
      public readonly string Message;
      public readonly Exception Exception;

      #endregion

      #region Constructor

      public Entry( EntryType type, string name, string message = null, Exception exception = null )
      {
        Type = type;
        Name = name;
        Message = message;
        Exception = exception;
      }

      #endregion

    }

    #endregion

  }

}
