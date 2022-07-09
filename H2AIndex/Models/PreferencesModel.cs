﻿using H2AIndex.Common;

namespace H2AIndex.Models
{

  public class PreferencesModel : ObservableObject
  {

    #region Properties

    public static PreferencesModel Default
    {
      get
      {
        return new PreferencesModel
        {
          TextureExportOptions = TextureExportOptionsModel.Default
        };
      }
    }

    public string H2ADirectoryPath { get; set; }
    public string DefaultExportPath { get; set; }

    public TextureExportOptionsModel TextureExportOptions { get; set; }

    #endregion

    #region Constructor

    public PreferencesModel()
    {
    }

    #endregion

  }

}
