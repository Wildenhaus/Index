using System;
using System.Windows.Input;

namespace Index.Common
{

  public class DelegateCommand : ICommand
  {

    #region Events

    public event EventHandler CanExecuteChanged;

    #endregion

    #region Data Members

    private readonly Predicate<object> _canExecute;
    private readonly Action<object> _execute;

    #endregion

    #region Constructor

    public DelegateCommand( Action<object> execute, Predicate<object> canExecute )
    {
      _execute = execute;
      _canExecute = canExecute;
    }

    public DelegateCommand( Action<object> execute )
      : this( execute, null )
    {
    }

    #endregion

    #region Public Methods

    public virtual bool CanExecute( object parameter )
    {
      if ( _canExecute == null )
        return true;

      return _canExecute( parameter );
    }

    public void Execute( object parameter )
      => _execute( parameter );

    public void RaiseCanExecuteChanged()
      => CanExecuteChanged?.Invoke( this, EventArgs.Empty );

    #endregion

  }

}
