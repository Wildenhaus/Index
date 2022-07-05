using System.ComponentModel;

namespace H2AIndex.Common
{

  public interface IProgressData : INotifyPropertyChanged
  {

    #region Properties

    string Status { get; }

    string SubStatus { get; }

    string UnitName { get; }

    int CompletedUnits { get; }

    int TotalUnits { get; }

    double PercentageComplete { get; }

    bool IsIndeterminate { get; }

    #endregion

  }

}
