using System.Threading.Tasks;
using H2AIndex.Common;

namespace H2AIndex.Processes
{

  public interface IProcess : IProgressData
  {

    #region Properties

    ProcessState State { get; }

    StatusList StatusList { get; }

    #endregion

    #region Public Methods

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
