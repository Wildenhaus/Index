using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Reclaimer.Models;
using Saber3D.Files;
using Studio.Controls;

namespace Reclaimer.Saber3D.H2A.Controls
{

  /// <summary>
  /// Interaction logic for FileTree.xaml
  /// </summary>
  public partial class FileTree : UserControl
  {

    #region Data Members

    private readonly H2AFileContext FileContext;

    private TreeItemModel _rootNode;

    #endregion

    #region Properties

    public TabModel TabModel { get; }

    #endregion

    #region Dependency Properties

    private static readonly DependencyPropertyKey HasGlobalHandlersPropertyKey =
        DependencyProperty.RegisterReadOnly( nameof( HasGlobalHandlers ), typeof( bool ), typeof( FileTree ), new PropertyMetadata( false ) );

    public static readonly DependencyProperty HasGlobalHandlersProperty = HasGlobalHandlersPropertyKey.DependencyProperty;

    public bool HasGlobalHandlers
    {
      get => ( bool ) GetValue( HasGlobalHandlersProperty );
      private set => SetValue( HasGlobalHandlersPropertyKey, value );
    }

    #endregion

    #region Constructor

    public FileTree( H2AFileContext fileContext )
    {
      InitializeComponent();

      FileContext = fileContext;

      TabModel = new TabModel( this, TabItemType.Tool );
      DataContext = this;

      _rootNode = new TreeItemModel( "H2A Files" );
      tv.ItemsSource = _rootNode.Items;

      TabModel.Header = "H2A File Tree";

      Refresh();
    }

    #endregion

    #region Properties

    public void Refresh()
    {
      BuildItemTree();
    }

    #endregion

    #region Private Methods

    private void BuildItemTree()
    {
      var result = new List<TreeItemModel>();
      var groups = FileContext.Files.Values
        .GroupBy( x => Path.GetExtension( x.Name ) );

      foreach ( var group in groups.OrderBy( x => x.Key ) )
      {
        var node = new TreeItemModel { Header = group.Key };
        foreach ( var file in group.OrderBy( x => x.Name ) )
        {
          node.Items.Add( new TreeItemModel
          {
            Header = file.Name,
            Tag = file
          } );
        }
        result.Add( node );
      }

      _rootNode.Items.Reset( result );
    }

    private void RecursiveCollapseNode( TreeItemModel node )
    {
      foreach ( var n in node.Items )
        RecursiveCollapseNode( n );
      node.IsExpanded = false;
    }

    #endregion

    #region Event Handlers

    private void btnCollapseAll_Click( object sender, RoutedEventArgs e )
    {
      foreach ( var node in _rootNode.Items )
        RecursiveCollapseNode( node );
    }

    private void TreeItemPreviewMouseRightButtonDown( object sender, MouseButtonEventArgs e )
      => ( sender as TreeViewItem ).IsSelected = true;

    private void TreeItemMouseDoubleClick( object sender, MouseButtonEventArgs e )
    {
      return;
    }

    private void TreeItemContextMenuOpening( object sender, ContextMenuEventArgs e )
    {
      return;
    }

    private void ContextItem_Click( object sender, RoutedEventArgs e )
    {
      return;
    }

    #endregion

  }

}
