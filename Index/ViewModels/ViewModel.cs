using System;
using PropertyChanged;

namespace Index.ViewModels
{

  [AddINotifyPropertyChangedInterface]
  public class ViewModel : IDisposable
  {

    #region Data Members

    private bool _isDisposed;

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
