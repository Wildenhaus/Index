using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using H2AIndex.Common.Extensions;

namespace H2AIndex.UI.Modals
{

  public abstract class Modal : ContentControl, IModal
  {

    #region Events

    public event EventHandler<Modal> ModalClosing;

    #endregion

    #region Data Members

    private bool _isDisposed;

    private TaskCompletionSource<object> _tcs;

    #endregion

    #region Properties

    public Task<object> Task
    {
      get => _tcs.Task;
    }

    #endregion

    #region Constructor

    protected Modal()
    {
      _tcs = new TaskCompletionSource<object>();

      Opacity = 0;
    }

    ~Modal()
    {
      Dispose( false );
    }

    #endregion

    #region Public Methods

    public Task Show()
      => AnimateFade( 0, 1 );

    public Task Hide()
      => AnimateFade( 1, 0 );

    #endregion

    #region Private Methods

    protected virtual void CloseModal()
      => CloseModal( null );

    protected virtual void CloseModal( object result )
    {
      _tcs.SetResult( result );
      Dispose();
    }

    private Task AnimateFade( double startValue, double endValue )
    {
      Opacity = startValue;

      var storyboard = new Storyboard();

      var animation = new DoubleAnimation()
      {
        From = startValue,
        To = endValue,
        Duration = TimeSpan.FromMilliseconds( 100 ),
        FillBehavior = FillBehavior.HoldEnd
      };

      storyboard.Children.Add( animation );

      Storyboard.SetTarget( animation, this );
      Storyboard.SetTargetProperty( animation, new PropertyPath( OpacityProperty ) );

      return storyboard.BeginAsync();
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
        ModalClosing?.Invoke( this, this );
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