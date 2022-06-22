using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using H2AIndex.Common.Enumerations;
using H2AIndex.Services;
using Microsoft.Extensions.DependencyInjection;

namespace H2AIndex.Controls
{

  public partial class FileBrowserBox : UserControl
  {

    #region Data Members

    public static readonly DependencyProperty DialogActionTypeProperty = DependencyProperty.Register(
     nameof( DialogActionType ),
     typeof( FileDialogActionType ),
     typeof( FileBrowserBox ),
     new PropertyMetadata( FileDialogActionType.Open ) );

    public static readonly DependencyProperty DialogPathTypeProperty = DependencyProperty.Register(
      nameof( DialogPathType ),
      typeof( FileDialogPathType ),
      typeof( FileBrowserBox ),
      new PropertyMetadata( FileDialogPathType.File ) );

    public static readonly DependencyProperty DialogTitleProperty = DependencyProperty.Register(
      nameof( DialogTitle ),
      typeof( string ),
      typeof( FileBrowserBox ) );

    public static readonly DependencyProperty PathProperty = DependencyProperty.Register(
      nameof( Path ),
      typeof( string ),
      typeof( FileBrowserBox ) );

    public static readonly DependencyProperty IsValidPathProperty = DependencyProperty.Register(
      nameof( IsValidPath ),
      typeof( bool ),
      typeof( FileBrowserBox ) );

    #endregion

    #region Properties

    public FileDialogActionType DialogActionType
    {
      get => ( FileDialogActionType ) GetValue( DialogActionTypeProperty );
      set => SetValue( DialogActionTypeProperty, value );
    }

    public FileDialogPathType DialogPathType
    {
      get => ( FileDialogPathType ) GetValue( DialogPathTypeProperty );
      set => SetValue( DialogPathTypeProperty, value );
    }

    public string DialogTitle
    {
      get => ( string ) GetValue( DialogTitleProperty );
      set => SetValue( DialogTitleProperty, value );
    }

    public bool IsValidPath
    {
      get => ( bool ) GetValue( IsValidPathProperty );
      set => SetValue( IsValidPathProperty, value );
    }

    public string Path
    {
      get => ( string ) GetValue( PathProperty );
      set => SetValue( PathProperty, value );
    }

    private IFileDialogService DialogService
    {
      get
      {
        var serviceProvider = ( ( App ) App.Current ).ServiceProvider;
        return serviceProvider.GetRequiredService<IFileDialogService>();
      }
    }

    #endregion

    #region Constructor

    public FileBrowserBox()
    {
      InitializeComponent();
    }

    #endregion

    #region Private Methods

    private async Task<string> BrowseForDirectory()
    {
      return await DialogService.BrowseForDirectory( title: DialogTitle );
    }

    private async Task<string> BrowseForFile()
    {
      switch ( DialogActionType )
      {
        case FileDialogActionType.Open:
          var result = await DialogService.BrowseForOpenFile( title: DialogTitle, multiselect: false );
          return result?.FirstOrDefault();

        case FileDialogActionType.Save:
          return await DialogService.BrowseForSaveFile( title: DialogTitle );

        default:
          return null;
      }
    }

    #endregion

    #region Event Handlers

    private async void OnBrowseButtonClick( object sender, RoutedEventArgs e )
    {
      string path = null;
      switch ( DialogPathType )
      {
        case FileDialogPathType.File:
          path = await BrowseForFile();
          break;

        case FileDialogPathType.Directory:
          path = await BrowseForDirectory();
          break;
      }

      if ( string.IsNullOrWhiteSpace( path ) )
        return;

      Path = path;
    }

    #endregion

    #region Event Handlers

    private void OnPathChanged( object sender, TextChangedEventArgs e )
    {
      var newPath = Path = PathTextBox.Text;
      IsValidPath = File.Exists( newPath ) || Directory.Exists( newPath );
    }

    #endregion

  }

}
