using System;
using Index.Controls;
using PropertyChanged;

namespace Index.Modals
{

  public partial class LoadingModal : HostedModal
  {

    #region Properties

    public LoadingModalViewModel ViewModel { get; }

    #endregion

    #region Constructor

    public LoadingModal( ContentHost host )
      : base( host )
    {
      InitializeComponent();
      DataContext = ViewModel = new LoadingModalViewModel();
    }

    #endregion

  }

  [AddINotifyPropertyChangedInterface]
  public class LoadingModalViewModel
  {

    public string Header { get; set; }

    public string UnitName { get; set; }

    public int CompletedUnits { get; set; }

    public int TotalUnits { get; set; }

    public bool IsIndeterminate { get; set; }

    [DependsOn( nameof( CompletedUnits ) )]
    public string SubHeader
    {
      get
      {
        if ( IsIndeterminate ) return null;
        return $"{CompletedUnits} of {TotalUnits} {UnitName} ({Percentage:0.00%})";
      }
    }

    [DependsOn( nameof( CompletedUnits ) )]
    public double Percentage
    {
      get => ( double ) CompletedUnits / Math.Max( 1, TotalUnits );
    }

  }

}
