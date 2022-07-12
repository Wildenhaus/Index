using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using H2AIndex.ViewModels;

namespace H2AIndex.Views
{

  public partial class ModelView : View<ModelViewModel>
  {

    #region Constructor

    public ModelView()
    {
      InitializeComponent();
      RemoveDirectionalViewKeyBindings();
      DataContextChanged += OnDataContextChanged;
    }

    #endregion

    #region Overrides

    protected override void OnDisposing()
    {
      DataContextChanged -= OnDataContextChanged;
      base.OnDisposing();
    }

    #endregion

    #region Private Methods

    private void RemoveDirectionalViewKeyBindings()
    {
      var toRemove = new List<KeyBinding>();
      foreach ( var binding in Viewport.InputBindings )
        if ( binding is KeyBinding keyBinding )
          toRemove.Add( keyBinding );

      foreach ( var binding in toRemove )
        Viewport.InputBindings.Remove( binding );
    }

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

    private void OnDataContextChanged( object sender, DependencyPropertyChangedEventArgs e )
    {
      if ( DataContext is ModelViewModel viewModel )
        viewModel.Viewport = Viewport;
    }

    private void OnListViewSizeChanged( object sender, SizeChangedEventArgs e )
      => UpdateColumnsWidth( sender as ListView );

    private void OnListViewLoaded( object sender, RoutedEventArgs e )
      => UpdateColumnsWidth( sender as ListView );

    #endregion

  }

}
