using System;

namespace Index.ViewModels
{

  public class LoadingViewModel : ViewModel, IDisposable
  {

    #region Events

    public event EventHandler Disposing;

    #endregion

    #region Data Members

    private bool _isDisposed;

    #endregion

    #region Properties

    private string _title;
    public string Title
    {
      get => _title;
      set => SetProperty( ref _title, value );
    }

    private bool _isIndeterminate;
    public bool IsIndeterminate
    {
      get => _isIndeterminate;
      set => SetProperty( ref _isIndeterminate, value );
    }

    public string SubTitle
    {
      get => $"{_unitsCompleted} of {_totalUnits} ({Progress:0.00}%)";
    }

    public double Progress
    {
      get => ( double ) _unitsCompleted / _totalUnits * 100;
    }

    private bool _completed;
    public bool Completed
    {
      get => _completed;
      set => SetProperty( ref _completed, value );
    }

    private int _unitsCompleted;
    public int UnitsCompleted
    {
      get => _unitsCompleted;
      set
      {
        SetProperty( ref _unitsCompleted, value );

        IsIndeterminate = false;
        RaisePropertyChanged( nameof( SubTitle ) );
        RaisePropertyChanged( nameof( Progress ) );
      }
    }

    private int _totalUnits;
    public int TotalUnits
    {
      get => _totalUnits;
      set
      {
        SetProperty( ref _totalUnits, value );

        IsIndeterminate = false;
        RaisePropertyChanged( nameof( SubTitle ) );
        RaisePropertyChanged( nameof( Progress ) );
      }
    }

    #endregion

    #region IDisposable Methods

    public void Dispose()
    {
      if ( _isDisposed )
        return;

      Completed = true;

      Disposing?.Invoke( this, EventArgs.Empty );
      _isDisposed = true;
    }

    #endregion

  }

}
