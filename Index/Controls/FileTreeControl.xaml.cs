using System.Windows.Controls;
using DebounceThrottle;
using Index.ViewModels;

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

    private void SearchBox_TextChanged( object sender, TextChangedEventArgs e )
    {
      _searchDebouncer.Debounce( () =>
      {
        string searchTerm = null;
        Dispatcher.Invoke( () => { searchTerm = SearchBox.Text; } );

        _viewModel.FilterFiles( searchTerm );

        if ( string.IsNullOrWhiteSpace( searchTerm ) )
          CollapseAllGroups();
      } );
    }

    private void CollapseAllGroups()
    {
      Dispatcher.Invoke( () =>
      {
        foreach ( TreeViewItem item in FileTree.Items )
          item.IsExpanded = false;
      } );
    }

  }

}
