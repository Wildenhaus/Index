using System.Windows.Input;
using Index.Common;
using Index.UI.Controls;
using Index.ViewModels;

namespace Index.Modals
{

  public partial class PreferencesModal : WindowedModal
  {

    private PreferencesViewModel _prefs;

    public ICommand BrowseForDirectoryCommand { get; }

    public PreferencesModal( ContentHost host )
      : base( host )
    {
      BrowseForDirectoryCommand = new DelegateCommand( BrowseForDirectory );

      InitializeComponent();
      DataContext = _prefs = PreferencesManager.Preferences;
      ModelFormats.ItemsSource = PreferencesManager.ExportFormats;
    }

    protected override void OnDisposing()
    {
      PreferencesManager.SavePreferences();
    }

    #region Event Handlers

    private void BrowseForDirectory( object param )
    {
      var propertyName = param as string;
      var property = _prefs.GetType().GetProperty( propertyName );

      var path = AppManager.BrowseForDirectory(
        title: "Select H2A Game Directory",
        defaultPath: _prefs.H2AGameDirectory );

      if ( string.IsNullOrWhiteSpace( path ) )
        return;

      property.SetValue( _prefs, path );
    }

    #endregion

  }

}
