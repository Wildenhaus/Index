using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using H2AIndex.Models;

namespace H2AIndex.Services
{

  public class PreferencesService : IPreferencesService
  {

    #region Data Members

    private static readonly AutoResetEvent _ioLock = new AutoResetEvent( true );

    #endregion

    #region Properties

    public PreferencesModel Preferences { get; private set; }

    #endregion

    #region Public Methods

    public async Task Initialize()
    {
      if ( Preferences != null )
        return;

      await LoadPreferences();

      if ( Preferences is null )
      {
        Preferences = PreferencesModel.Default;
        await SavePreferences();
      }
      else
        EnsurePreferencesAreSet();
    }

    public async Task<PreferencesModel> LoadPreferences()
    {
      try
      {
        _ioLock.WaitOne();

        using var fs = File.OpenRead( GetPreferencesPath() );
        Preferences = await JsonSerializer.DeserializeAsync<PreferencesModel>( fs );
      }
      catch ( Exception ex )
      {
      }
      finally
      {
        _ioLock.Set();
      }

      return Preferences;
    }

    public async Task SavePreferences()
    {
      try
      {
        _ioLock.WaitOne();

        using var fs = File.Create( GetPreferencesPath() );
        await JsonSerializer.SerializeAsync( fs, Preferences );
      }
      catch ( Exception ex )
      {
      }
      finally
      {
        _ioLock.Set();
      }
    }

    #endregion

    #region Private Methods

    private string GetPreferencesPath()
    {
      var executableDir = AppDomain.CurrentDomain.BaseDirectory;
      return Path.Combine( executableDir, "H2AIndex.prefs" );
    }

    private void EnsurePreferencesAreSet()
    {
      if ( Preferences.ModelExportOptions is null )
        Preferences.ModelExportOptions = ModelExportOptionsModel.Default;
      if ( Preferences.TextureExportOptions is null )
        Preferences.TextureExportOptions = TextureExportOptionsModel.Default;
    }

    #endregion

  }

}
