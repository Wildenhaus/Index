using System;
using System.Threading.Tasks;
using System.Windows.Input;
using H2AIndex.Common;
using H2AIndex.Views;

namespace H2AIndex.Models
{

  public class Tab : ITab
  {

    #region Events

    public event EventHandler CloseRequested;
    public event EventHandler CloseAllRequested;
    public event EventHandler CloseAllButThisRequested;

    #endregion

    #region Data Members

    private bool _isDisposed;

    #endregion

    #region Properties

    public string Name { get; }
    public IView View { get; }

    public ICommand CloseCommand { get; }
    public ICommand CloseAllCommand { get; }
    public ICommand CloseAllButThisCommand { get; }

    #endregion

    #region Constructor

    public Tab( string name, IView view )
    {
      Name = name;
      View = view;

      CloseCommand = new Command( Close );
      CloseAllCommand = new Command( CloseAll );
      CloseAllButThisCommand = new Command( CloseAllButThis );
    }

    #endregion

    #region Public Methods

    public void Close()
      => CloseRequested?.Invoke( this, EventArgs.Empty );

    public void CloseAll()
      => CloseAllRequested( this, EventArgs.Empty );

    public void CloseAllButThis()
      => CloseAllButThisRequested( this, EventArgs.Empty );

    #endregion

    #region Private Methods

    protected virtual Task OnInitializing()
      => Task.CompletedTask;

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
      View?.Dispose();
    }

    #endregion

  }

}
