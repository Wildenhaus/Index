using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Assimp;
using Index.ViewModels;

namespace Index
{

  public static class PreferencesManager
  {

    private static PreferencesViewModel _preferences;

    public static ExportFormatDescription[] ExportFormats { get; private set; }

    public static PreferencesViewModel Preferences => _preferences;

    static PreferencesManager()
    {
      GetModelExportFormats();
      _preferences = LoadPreferences();
    }

    private static PreferencesViewModel LoadPreferences()
    {
      var prefPath = GetPreferencesPath();
      if ( !File.Exists( prefPath ) )
        return PreferencesViewModel.Defaults;

      var jsonData = File.ReadAllText( prefPath );
      var prefs = JsonSerializer.Deserialize<PreferencesViewModel>( jsonData );
      if ( prefs is null )
        prefs = PreferencesViewModel.Defaults;

      return prefs;
    }

    public static void SavePreferences()
    {
      var prefPath = GetPreferencesPath();

      var jsonData = JsonSerializer.Serialize( _preferences );
      File.WriteAllText( prefPath, jsonData );
    }

    private static void GetModelExportFormats()
    {
      using ( var ctx = new AssimpContext() )
      {
        ExportFormats = ctx.GetSupportedExportFormats().OrderBy( x => x.FileExtension ).ToArray();
      }
    }

    private static string GetPreferencesPath()
    {
      var executableDir = AppDomain.CurrentDomain.BaseDirectory;
      return Path.Combine( executableDir, "IndexPrefs.json" );
    }

  }

}
