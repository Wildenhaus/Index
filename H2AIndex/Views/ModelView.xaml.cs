using System.Windows;
using System.Windows.Controls;
using H2AIndex.ViewModels;

namespace H2AIndex.Views
{

  public partial class ModelView : View<ModelViewModel>
  {

    #region Constructor

    public ModelView()
    {
      InitializeComponent();
    }

    #endregion

    #region Overrides

    protected override void OnDisposing()
    {
      ModelViewer?.Dispose();
      base.OnDisposing();
    }

    #endregion

    #region Private Methods

    private void UpdateColumnsWidth( ListView listView )
    {
      if ( listView is null )
        return;

      var gridView = listView.View as GridView;
      if ( gridView is null )
        return;

      var lastColumnIdx = gridView.Columns.Count - 1;
      if ( listView.ActualWidth == double.NaN )
        listView.Measure( new Size( double.PositiveInfinity, double.PositiveInfinity ) );

      var remainingSpace = listView.ActualWidth;
      for ( int i = 0; i < gridView.Columns.Count; i++ )
        if ( i != lastColumnIdx )
          remainingSpace -= gridView.Columns[ i ].ActualWidth;

      gridView.Columns[ lastColumnIdx ].Width = remainingSpace >= 0 ? remainingSpace : 0;
    }

    #endregion

    #region Event Handlers

    private void OnContextMenuLoaded( object sender, RoutedEventArgs e )
    {
      ( sender as ContextMenu ).DataContext = this.DataContext;
    }

    private void OnListViewSizeChanged( object sender, SizeChangedEventArgs e )
      => UpdateColumnsWidth( sender as ListView );

    private void OnListViewLoaded( object sender, RoutedEventArgs e )
      => UpdateColumnsWidth( sender as ListView );

    #endregion

  }

}
