using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace H2AIndex.Common
{

  public class Command : ICommand
  {

    #region Events

    public event EventHandler CanExecuteChanged;

    #endregion

    #region Data Members

    private readonly Action _execute;
    private readonly Func<bool> _canExecute;

    #endregion

    #region Constructor

    public Command( Action execute, Func<bool> canExecute = null )
    {
      _execute = execute;
      _canExecute = canExecute ?? ( () => true );
    }

    #endregion

    #region Public Methods

    public bool CanExecute( object parameter )
      => _canExecute();

    public void Execute( object parameter )
      => _execute();

    #endregion

  }

  public class Command<T> : ICommand
  {

    #region Events

    public event EventHandler CanExecuteChanged;

    #endregion

    #region Data Members

    private readonly Action<T> _execute;
    private readonly Func<T, bool> _canExecute;

    #endregion

    #region Constructor

    public Command( Action<T> execute, Func<T, bool> canExecute = null )
    {
      _execute = execute;
      _canExecute = canExecute ?? ( _ => true );
    }

    #endregion

    #region Public Methods

    public bool CanExecute( object parameter )
      => _canExecute( ( T ) parameter );

    public void Execute( object parameter )
      => _execute( ( T ) parameter );

    #endregion

  }

  public class AsyncCommand : ICommand
  {

    #region Events

    public event EventHandler CanExecuteChanged;

    #endregion

    #region Data Members

    private readonly Func<Task> _execute;
    private readonly Func<bool> _canExecute;

    private bool _isExecuting;

    #endregion

    #region Constructor

    public AsyncCommand( Func<Task> execute, Func<bool> canExecute = null )
    {
      _execute = execute;
      _canExecute = canExecute ?? ( () => true );
    }

    #endregion

    #region Public Methods

    public bool CanExecute( object parameter )
    {
      if ( _isExecuting )
        return false;

      return _canExecute();
    }

    public void Execute( object parameter )
    {
      _isExecuting = true;
      CanExecuteChanged?.Invoke( this, EventArgs.Empty );

      _execute().ContinueWith( t =>
      {
        _isExecuting = false;
        CanExecuteChanged?.Invoke( this, EventArgs.Empty );
      }, TaskScheduler.FromCurrentSynchronizationContext() );
    }

    #endregion

  }

  public class AsyncCommand<T> : ICommand
  {

    #region Events

    public event EventHandler CanExecuteChanged;

    #endregion

    #region Data Members

    private readonly Func<T, Task> _execute;
    private readonly Func<T, bool> _canExecute;

    private bool _isExecuting;

    #endregion

    #region Constructor

    public AsyncCommand( Func<T, Task> execute, Func<T, bool> canExecute = null )
    {
      _execute = execute;
      _canExecute = canExecute ?? ( _ => true );
    }

    #endregion

    #region Public Methods

    public bool CanExecute( object parameter )
    {
      if ( _isExecuting )
        return false;

      return _canExecute( ( T ) parameter );
    }

    public void Execute( object parameter )
    {
      _isExecuting = true;
      CanExecuteChanged?.Invoke( this, EventArgs.Empty );

      _execute( ( T ) parameter ).ContinueWith( t =>
      {
        _isExecuting = false;
        CanExecuteChanged?.Invoke( this, EventArgs.Empty );
      } );
    }

    #endregion

  }

}
