using System;
using System.Windows.Controls;
using Index.Controls;

namespace Index.Modals
{

  public abstract class HostedModal : UserControl, IDisposable
  {

    #region Data Members

    private ContentHost _host;
    private bool _isDisposed;

    #endregion

    #region Properties

    public ContentHost Host { get; }

    #endregion

    #region Constructor

    protected HostedModal( ContentHost host )
    {
      Host = host;
    }

    #endregion

    #region IDisposable Methods

    public void Dispose()
      => Dispose( true );

    private void Dispose( bool disposing = true )
    {
      if ( _isDisposed )
        return;

      if ( disposing )
      {
        Host.Remove( this );
        OnDisposing();
      }

      _isDisposed = true;
    }

    protected virtual void OnDisposing()
    {
    }

    #endregion

  }

}
