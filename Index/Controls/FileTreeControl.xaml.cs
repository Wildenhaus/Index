using System.Windows.Controls;
using DebounceThrottle;
using Index.ViewModels;
using Saber3D.Files;

namespace Index.Controls
{

  public partial class FileTreeControl : UserControl
  {

    #region Data Members

    private FileContextViewModel _viewModel;
    private DebounceDispatcher _searchDebouncer;

    #endregion

    public FileTreeControl()
    {
      _searchDebouncer = new DebounceDispatcher( 500 );
      InitializeComponent();

      _viewModel = new FileContextViewModel();
      this.DataContext = _viewModel;
    }

    public void Refresh()
      => _viewModel.Refresh();

    #region Event Handlers

    private void SearchBox_TextChanged( object sender, TextChangedEventArgs e )
    {
      _searchDebouncer.Debounce( () =>
      {
        string searchTerm = null;
        Dispatcher.Invoke( () => { searchTerm = SearchBox.Text; } );

        _viewModel.FilterFiles( searchTerm );

        if ( string.IsNullOrWhiteSpace( searchTerm ) )
          CollapseAllGroups();
        else
          ExpandAllGroups();
      } );
    }

    private void FileTree_MouseDoubleClick( object sender, System.Windows.Input.MouseButtonEventArgs e )
    {
      var item = FileTree.SelectedItem as IS3DFile;
      if ( item is null )
        return;

      AppManager.CreateViewForFile( item );
    }

    private void OnExpandAllGroupsClicked( object sender, System.Windows.RoutedEventArgs e )
    {
      ExpandAllGroups();
    }

    private void OnCollapseAllGroupsClicked( object sender, System.Windows.RoutedEventArgs e )
    {
      CollapseAllGroups();
    }

    #endregion

    #region Private Methods

    private void CollapseAllGroups()
    {
      Dispatcher.Invoke( () => ExpandCollapseInternal( FileTree, expand: false ) );
    }

    private void ExpandAllGroups()
    {
      Dispatcher.Invoke( () => ExpandCollapseInternal( FileTree, expand: true ) );
    }

    private void ExpandCollapseInternal( ItemsControl items, bool expand )
    {
      foreach ( var obj in items.Items )
      {
        var childControl = items.ItemContainerGenerator.ContainerFromItem( obj );

        if ( childControl is TreeViewItem treeItem )
          treeItem.IsExpanded = expand;
        else if ( childControl is ItemsControl itemControl )
          ExpandCollapseInternal( childControl as ItemsControl, expand );
      }
    }

    #endregion

  }

}
