using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Reclaimer.Plugins;
using Reclaimer.Saber3D.H2A.Controls;
using Reclaimer.Utilities;
using Saber3D.Files;

namespace Reclaimer.Saber3D.H2A
{

  public class H2APlugin : Plugin
  {

    #region Constants

    private const string KEY_OPEN = "H2A.Open.";

    private const string KEY_OPEN_PCK = KEY_OPEN + "Pck";
    private const string PATH_OPEN_PCK = @"File\H2A\Open Pck File";
    private const string DIALOG_FILTER_PCK = "H2A Pck File|*.pck";

    private const string KEY_OPEN_DIR = KEY_OPEN + "Dir";
    private const string PATH_OPEN_DIR = @"File\H2A\Open H2A Directory";


    #endregion

    #region Data Members

    private FileTree _fileTree;

    private static H2APluginSettings Settings;
    private static H2AFileContext FileContext;

    #endregion

    #region Properties

    public override string Name => "Saber3D File Support";

    #endregion

    #region Overrides

    public override IEnumerable<PluginMenuItem> GetMenuItems()
    {
      yield return new PluginMenuItem( KEY_OPEN_PCK, PATH_OPEN_PCK, OnMenuItemClick );
      yield return new PluginMenuItem( KEY_OPEN_DIR, PATH_OPEN_DIR, OnMenuItemClick );
    }

    public override void Initialise()
    {
      Settings = LoadSettings<H2APluginSettings>();
      FileContext = new H2AFileContext();
    }

    public override void Suspend()
    {
      SaveSettings( Settings );
      // TODO: Dispose H2AFileContext
    }

    public override bool SupportsFileExtension( string extension )
    {
      switch ( extension.ToLower() )
      {
        case ".pck":
        case ".tpl":
        case ".lg":
        case ".pct":
          return true;
        default:
          return false;
      }
    }

    #endregion

    #region Event Handlers

    private void OnMenuItemClick( string key )
    {
      if ( key.StartsWith( KEY_OPEN ) )
      {
        OnOpenMenuItemClick( key );
        return;
      }
    }

    private void OnOpenMenuItemClick( string key )
    {
      switch ( key )
      {
        case KEY_OPEN_PCK:
          OpenPckFile();
          break;
        case KEY_OPEN_DIR:
          OpenDirectory();
          break;
      }
    }

    #endregion

    #region Private Methods

    private void OpenPckFile()
    {
      var dialog = new OpenFileDialog
      {
        Title = "Open H2A Pck File",
        Filter = DIALOG_FILTER_PCK,
        Multiselect = true,
        CheckFileExists = true
      };

      if ( !string.IsNullOrWhiteSpace( Settings.DefaultDirectory ) )
        dialog.InitialDirectory = Settings.DefaultDirectory;

      if ( dialog.ShowDialog() != true )
        return;

      try
      {
        LogOutput( "Loading H2A Pck File(s)..." );
        var filesLoaded = 0;
        foreach ( var fileName in dialog.FileNames )
          if ( FileContext.OpenFile( fileName ) )
            filesLoaded++;

        if ( filesLoaded == 0 )
        {
          Substrate.ShowErrorMessage( "No valid files were found." );
          return;
        }

        LogOutput( $"{filesLoaded} Pck File(s) Loaded." );
        ShowFileTree();
      }
      catch ( Exception ex )
      {
        Substrate.ShowErrorMessage( $"Load failed:\n{ex.Message}" );
        LogError( "Load Failed.", ex );
      }
    }

    private void OpenDirectory()
    {
      var dialog = new FolderSelectDialog
      {
        Title = "Open H2A Directory",
      };

      if ( !string.IsNullOrWhiteSpace( Settings.DefaultDirectory ) )
        dialog.InitialDirectory = Settings.DefaultDirectory;

      if ( dialog.ShowDialog() != true )
        return;

      try
      {
        LogOutput( "Loading H2A Directory..." );
        if ( !FileContext.OpenDirectory( dialog.SelectedPath ) )
        {
          Substrate.ShowErrorMessage( "No valid files were found." );
          return;
        }

        LogOutput( "H2A Directory Loaded." );
        ShowFileTree();
      }
      catch ( Exception ex )
      {
        Substrate.ShowErrorMessage( $"Load failed:\n{ex.Message}" );
        LogError( "Load Failed.", ex );
      }
    }

    private void ShowFileTree()
    {
      var tabId = "H2A::FileTree";
      if ( Substrate.ShowTabById( tabId ) )
      {
        _fileTree.Refresh();
        return;
      }

      //try
      //{
      LogOutput( "Initializing H2A FileTree..." );
      _fileTree = new FileTree( FileContext );
      _fileTree.TabModel.ContentId = tabId;

      Substrate.AddTool( _fileTree.TabModel, Substrate.GetHostWindow(), Dock.Left, new GridLength( 400 ) );
      //}
      //catch ( Exception ex )
      //{
      //  LogError( "Failed to initialize H2A FileTree.", ex );
      //}
    }

    #endregion

  }

}
