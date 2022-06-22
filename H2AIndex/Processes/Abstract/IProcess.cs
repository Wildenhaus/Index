using System;
using System.Threading.Tasks;
using H2AIndex.Common;

namespace H2AIndex.Processes
{

  public interface IProcess : IProgressData
  {

    #region Events

    public event EventHandler Completed;
    public event EventHandler Error;

    #endregion

    #region Properties

    bool CanCancel { get; }

    ProcessState State { get; }

    StatusList StatusList { get; }

    Task CompletionTask { get; }

    #endregion

    #region Public Methods

    void Cancel();

    Task Execute();

    #endregion

  }

  public interface IProcess<TResult> : IProcess
  {

    #region Properties

    TResult Result { get; }

    #endregion

  }

}
