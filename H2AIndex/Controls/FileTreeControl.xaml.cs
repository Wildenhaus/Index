using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace H2AIndex.Models
{

  public partial class FileTreeControl : UserControl
  {

    #region Data Members

    public static readonly DependencyProperty FileDoubleClickCommandProperty = DependencyProperty.Register(
      nameof( FileDoubleClickCommand ),
      typeof( ICommand ),
      typeof( FileTreeControl ),
      new PropertyMetadata() );

    #endregion

    #region Properties

    public ICommand FileDoubleClickCommand
    {
      get => ( ICommand ) GetValue( FileDoubleClickCommandProperty );
      set => SetValue( FileDoubleClickCommandProperty, value );
    }

    #endregion

    #region Constructor

    public FileTreeControl()
    {
      InitializeComponent();
    }

    #endregion

    #region Private Methods

    private TreeViewItem TryGetClickedItem( MouseButtonEventArgs e )
    {
      var hit = e.OriginalSource as DependencyObject;
      while ( hit != null && !( hit is TreeViewItem ) )
        hit = VisualTreeHelper.GetParent( hit );

      return hit as TreeViewItem;
    }

    #endregion

    #region Event Handlers

    private void OnPreviewMouseDoubleClick( object sender, MouseButtonEventArgs e )
    {
      var item = TryGetClickedItem( e );
      if ( item is null )
        return;

      var fileModel = item.Header as FileModel;
      if ( fileModel is null )
        return;

      e.Handled = true;

      FileDoubleClickCommand?.Execute( fileModel );
    }

    #endregion

  }

}
