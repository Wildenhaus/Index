using System;
using System.Collections.Generic;
using System.IO;
using Reclaimer.Plugins;

namespace Reclaimer.Saber3D
{

  public class Saber3DPlugin : Plugin
  {

    #region Constants

    private const string KEY_OPENPCK = "KEY_OPENPCK";

    #endregion

    #region Properties

    public override string Name => "Saber3D Engine Support";

    private Settings Settings { get; set; }

    #endregion

    #region Overrides

    public override void Initialise()
    {
      Settings = LoadSettings<Settings>();
    }

    public override IEnumerable<PluginMenuItem> GetMenuItems()
    {
      yield return new PluginMenuItem( KEY_OPENPCK, "Saber3D\\Open .pck", OnMenuItemClicked );
    }

    public override bool CanOpenFile( OpenFileArgs args )
    {
      var fileExtension = Path.GetExtension( args.FileName );
      return IsSupportedFileExtension( fileExtension );
    }

    public override void OpenFile( OpenFileArgs args )
    {
      throw new NotImplementedException();
    }

    #endregion

    #region Event Handlers

    private void OnMenuItemClicked( string key )
    {
      switch ( key )
      {
        case KEY_OPENPCK:
          throw new NotImplementedException();
        default:
          return;
      }
    }

    #endregion

    #region Private Methods

    private bool IsSupportedFileExtension( string ext )
    {
      ext = ext.ToLower();
      switch ( ext )
      {
        case ".pck":
        case ".tpl":
          return true;
        default:
          return false;
      }
    }

    #endregion

  }

}
