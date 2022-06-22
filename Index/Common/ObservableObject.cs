using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Index.Common
{

  public interface IObservableObject : INotifyPropertyChanged
  {
  }

  public abstract class ObservableObject : IObservableObject
  {

    #region Events

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    #region Properties

    protected virtual bool ThrowOnInvalidPropertyName { get; set; }

    #endregion

    #region Private Methods

    protected void NotifyPropertyChanged( [CallerMemberName] string propertyName = null )
    {
      AssertValidPropertyName( propertyName );
      PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
    }

    [Conditional( "DEBUG" )]
    [DebuggerStepThrough]
    private void AssertValidPropertyName( string propertyName )
    {
      if ( TypeDescriptor.GetProperties( this )[ propertyName ] is null )
      {
        var message = $"Invalid Property Name: '{propertyName}'";
        if ( ThrowOnInvalidPropertyName )
          throw new ArgumentException( message );
        else
          Debug.Fail( message );
      }
    }

    #endregion

  }

}
