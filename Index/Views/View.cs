using System.Threading.Tasks;
using System.Windows.Controls;
using Index.UI.Controls;

namespace Index.Views
{

  public class View : UserControl
  {

    #region Data Members

    private ContentHost _host;

    private bool _isInitialized;
    private bool _isDisposed;

    #endregion

    #region Properties

    public ContentHost Host => _host;

    #endregion

    #region Constructor

    protected View()
    {
    }

    #endregion

    #region Public Methods

    public async Task Initialize( ContentHost host )
    {
      if ( _isInitialized )
        return;

      _host = host;

      await OnInitializing();

      _isInitialized = true;
    }

    #endregion

    #region Private Methods

    protected virtual Task OnInitializing()
      => Task.CompletedTask;

    protected virtual void OnDisposing()
    {
    }

    #endregion

    #region IDisposable Methods

    public void Dispose()
    {
      if ( _isDisposed )
        return;

      OnDisposing();

      _isDisposed = true;
    }

    #endregion

  }

}
