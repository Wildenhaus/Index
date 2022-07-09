using System;
using System.Threading;

namespace Index.Common
{

  // Based on https://gist.github.com/mstepura/7ded78c66927114aa8fa

  public class ActionThrottler : IDisposable
  {

    #region Constants

    private const int STATE_IDLE = 0;
    private const int STATE_EXECUTING = 1;
    private const int STATE_DISPOSING = 2;

    #endregion

    #region Data Members

    private readonly Action _action;
    private readonly TimeSpan _delay;
    private readonly Timer _timer;

    private int _state;

    #endregion

    #region Constructor

    public ActionThrottler( Action action, TimeSpan delay )
    {
      _action = action;
      _delay = delay;
      _timer = new Timer( OnExecute, null, Timeout.Infinite, Timeout.Infinite );

      _state = STATE_IDLE;
    }

    public ActionThrottler( Action action, int delayMilliseconds )
      : this( action, TimeSpan.FromMilliseconds( delayMilliseconds ) )
    {
    }

    #endregion

    #region Public Methods

    public void Execute()
    {
      var currentState = Interlocked.CompareExchange( ref _state, STATE_EXECUTING, STATE_IDLE );
      if ( currentState == STATE_IDLE )
        _timer.Change( _delay, Timeout.InfiniteTimeSpan );
    }

    #endregion

    #region Private Methods

    private void OnExecute( object state )
    {
      try
      {
        _action();
      }
      finally
      {
        Interlocked.CompareExchange( ref _state, STATE_IDLE, STATE_EXECUTING );
      }
    }

    #endregion

    #region IDisposable Methods

    public void Dispose()
    {
      var state = Interlocked.Exchange( ref _state, STATE_DISPOSING );
      if ( state == STATE_IDLE )
      {
        _timer.Dispose();
      }
      else if ( state == STATE_EXECUTING )
      {
        using ( var waitHandle = new ManualResetEvent( false ) )
        {
          if ( _timer.Dispose( waitHandle ) )
            waitHandle.WaitOne();
        }
      }
    }

    #endregion

  }

}
