using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using Index.UI.Controls;

namespace Index.Modals
{

  public abstract class HostedModal : ContentControl, IDisposable
  {

    #region Data Members

    private ContentHost _host;
    private bool _isDisposed;
    private TaskCompletionSource<string> _tcs;

    #endregion

    #region Properties

    public ContentHost Host { get; }

    public Task<string> AwaiterTask
    {
      get => _tcs.Task;
    }

    #endregion

    #region Constructor

    protected HostedModal( ContentHost host )
    {
      Host = host;

      _tcs = new TaskCompletionSource<string>();
    }

    #endregion

    #region Private Methods

    protected void CloseModal( string result = null )
    {
      _tcs.TrySetResult( result );
      Dispose();
    }

    protected void CloseModal( object sender )
    {
      string result = null;
      if ( sender is Button button && button.Content is string buttonContent )
        result = buttonContent;

      CloseModal( result );
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
