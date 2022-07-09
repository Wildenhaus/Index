using System;
using System.ComponentModel;
using System.Threading.Tasks;
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

    #endregion

    #region Properties

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
    }

    #endregion

    #region Public Methods

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
      }
      finally
      {
        _tcs.TrySetResult();
      }
    }

    #endregion

    #region Private Methods

    protected virtual Task OnInitializing()
      => Task.CompletedTask;

    protected abstract Task OnExecuting();

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
        State = ProcessState.Faulted;
        _isCompleted = true;
        RaiseErrorEvent( ex );
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
