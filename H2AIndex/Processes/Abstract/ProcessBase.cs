using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using H2AIndex.Common;
using PropertyChanged;

namespace H2AIndex.Processes
{

  [AddINotifyPropertyChangedInterface]
  public abstract class ProcessBase : IProcess
  {

    #region Events

    public event EventHandler Completed;
    public event EventHandler Error;
    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    #region Data Members

    private bool _isInitialized;
    private bool _isCompleted;
    private TaskCompletionSource _tcs;
    private CancellationTokenSource _cancelCts;

    #endregion

    #region Properties

    public virtual bool CanCancel { get; }

    public string Status { get; protected set; }
    public string UnitName { get; protected set; }
    public int CompletedUnits { get; protected set; }
    public int TotalUnits { get; protected set; }
    public bool IsIndeterminate { get; protected set; }

    [DependsOn( nameof( CompletedUnits ) )]
    public virtual string SubStatus
    {
      get
      {
        if ( IsIndeterminate ) return null;
        return $"{CompletedUnits} of {TotalUnits} {UnitName} ({PercentageComplete:0.00%})";
      }
    }

    [DependsOn( nameof( CompletedUnits ) )]
    public virtual double PercentageComplete
    {
      get => ( double ) CompletedUnits / Math.Max( 1, TotalUnits );
    }


    public ProcessState State { get; private set; }
    public StatusList StatusList { get; }

    public Task CompletionTask
    {
      get => _tcs.Task;
    }

    public ICommand CancelCommand { get; }

    protected bool IsCancellationRequested
    {
      get => CancellationToken.IsCancellationRequested;
    }

    protected CancellationToken CancellationToken
    {
      get => _cancelCts.Token;
    }

    protected IServiceProvider ServiceProvider
    {
      get => ( ( App ) App.Current ).ServiceProvider;
    }

    #endregion

    #region Constructor

    protected ProcessBase()
    {
      State = ProcessState.Idle;
      StatusList = new StatusList();

      _isInitialized = false;
      _tcs = new TaskCompletionSource();
      _cancelCts = new CancellationTokenSource();

      CancelCommand = new FireOnceCommand( Cancel );
    }

    #endregion

    #region Public Methods

    public void Cancel()
    {
      _cancelCts.Cancel();
      StatusList.AddWarning( "Process", "Process canceled by user." );

      Status = "Finishing Current Operations";
      IsIndeterminate = true;
    }

    public async Task Execute()
    {
      if ( _isCompleted )
        return;

      await Initialize();

      if ( State == ProcessState.Faulted )
        return;

      try
      {
        State = ProcessState.Executing;
        await OnExecuting();
        State = ProcessState.Complete;
        RaiseCompletedEvent();
      }
      catch ( Exception ex )
      {
        State = ProcessState.Faulted;
        _isCompleted = true;
        RaiseErrorEvent( ex );
        StatusList.AddError( "Process", "An unknown error occured.", ex );
      }
      finally
      {
        _tcs.TrySetResult();
      }

      OnComplete();
    }

    #endregion

    #region Private Methods

    protected virtual Task OnInitializing()
      => Task.CompletedTask;

    protected abstract Task OnExecuting();

    protected virtual Task OnComplete()
      => Task.CompletedTask;

    protected void BindStatusToSubProcess( IProcess subProcess )
    {
      void OnSubProcessChanged( object sender, PropertyChangedEventArgs e )
      {
        var process = sender as IProcess;
        switch ( e.PropertyName )
        {
          case nameof( Status ):
            Status = process.Status;
            break;
          case nameof( UnitName ):
            UnitName = process.UnitName;
            break;
          case nameof( CompletedUnits ):
            CompletedUnits = process.CompletedUnits;
            break;
          case nameof( TotalUnits ):
            TotalUnits = process.TotalUnits;
            break;
          case nameof( IsIndeterminate ):
            IsIndeterminate = process.IsIndeterminate;
            break;
        }
      }

      void OnSubProcessFinished( object sender, EventArgs e )
      {
        subProcess.PropertyChanged -= OnSubProcessChanged;
        subProcess.Completed -= OnSubProcessFinished;
        subProcess.Error -= OnSubProcessFinished;
      }

      subProcess.PropertyChanged += OnSubProcessChanged;
      subProcess.Completed += OnSubProcessFinished;
      subProcess.Error += OnSubProcessFinished;
    }

    private async Task Initialize()
    {
      if ( _isInitialized )
        return;

      try
      {
        State = ProcessState.Initializing;
        await OnInitializing();
        State = ProcessState.Initialized;
      }
      catch ( Exception ex )
      {
        StatusList.AddError( "Initialization", ex );
        State = ProcessState.Faulted;
        _isCompleted = true;
        RaiseErrorEvent( ex );
        _tcs.TrySetResult();
      }
    }

    private void RaiseCompletedEvent()
    {
      Completed?.Invoke( this, EventArgs.Empty );
    }

    private void RaiseErrorEvent( Exception ex )
    {
      Error?.Invoke( this, EventArgs.Empty );
    }

    #endregion

  }

  public abstract class ProcessBase<TResult> : ProcessBase, IProcess<TResult>
  {

    #region Properties

    public abstract TResult Result { get; }

    #endregion

  }

}
