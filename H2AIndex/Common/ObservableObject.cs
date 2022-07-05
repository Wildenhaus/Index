using System;
using System.ComponentModel;
using PropertyChanged;

namespace H2AIndex.Common
{

  [AddINotifyPropertyChangedInterface]
  public abstract class ObservableObject : IDisposable, INotifyPropertyChanged
  {

    #region Events

#pragma warning disable CS0067
    public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067

    #endregion

    #region Data Members

    private bool _isDisposed;

    #endregion

    #region Constructor

    ~ObservableObject()
    {
      Dispose( false );
    }

    #endregion

    #region IDisposable Methods

    public void Dispose()
    {
      Dispose( true );
    }

    private void Dispose( bool disposing )
    {
      if ( _isDisposed )
        return;

      if ( disposing )
        OnDisposing();

      _isDisposed = true;
    }

    protected virtual void OnDisposing()
    {
    }

    #endregion

  }

}
