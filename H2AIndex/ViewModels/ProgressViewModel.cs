using System;
using H2AIndex.Common;
using PropertyChanged;

namespace H2AIndex.ViewModels
{

  public class ProgressViewModel : ViewModel, IProgressData
  {

    #region Properties

    public string Status { get; set; }

    public string UnitName { get; set; }

    public int CompletedUnits { get; set; }

    public int TotalUnits { get; set; }

    public bool IsIndeterminate { get; set; }

    [DependsOn( nameof( CompletedUnits ) )]
    public string SubStatus
    {
      get
      {
        if ( IsIndeterminate ) return null;
        return $"{CompletedUnits} of {TotalUnits} {UnitName} ({PercentageComplete:0.00%})";
      }
    }

    [DependsOn( nameof( CompletedUnits ) )]
    public double PercentageComplete
    {
      get => ( double ) CompletedUnits / Math.Max( 1, TotalUnits );
    }

    #endregion

    #region Constructor

    public ProgressViewModel( IServiceProvider serviceProvider )
      : base( serviceProvider )
    {
    }

    #endregion

  }

}
