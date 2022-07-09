using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using H2AIndex.Common;
using H2AIndex.Common.Enumerations;
using H2AIndex.Models;

namespace H2AIndex.ViewModels
{

  public class ModelExportOptionsViewModel : ViewModel
  {

    #region Properties

    public IReadOnlyList<ModelFileFormat> ModelFileFormats { get; set; }
    public IReadOnlyList<TextureFileFormat> TextureFileFormats { get; set; }
    public IReadOnlyList<NormalMapFormat> NormalMapFormats { get; set; }

    public bool IsForBatch { get; set; }
    public ModelExportOptionsModel Options { get; set; }

    public bool IsValidPath { get; set; }

    public ICommand ExplainFileFiltersCommand { get; }
    public ICommand ExplainTextureDefinitionsCommand { get; }
    public ICommand ExplainLODsCommand { get; }
    public ICommand ExplainVolumesCommand { get; }

    #endregion

    #region Constructor

    public ModelExportOptionsViewModel( IServiceProvider serviceProvider )
      : base( serviceProvider )
    {
      ExplainFileFiltersCommand = new AsyncCommand( ExplainFileFilters );
      ExplainTextureDefinitionsCommand = new AsyncCommand( ExplainTextureDefinitions );
      ExplainLODsCommand = new AsyncCommand( ExplainLODs );
      ExplainVolumesCommand = new AsyncCommand( ExplainVolumes );
    }

    #endregion

    #region Overrides

    protected override async Task OnInitializing()
    {
      ModelFileFormats = Enum.GetValues<ModelFileFormat>();
      TextureFileFormats = Enum.GetValues<TextureFileFormat>();
      NormalMapFormats = Enum.GetValues<NormalMapFormat>();

      Options = GetPreferences().ModelExportOptions;
    }

    #endregion

    #region Private Methods

    private async Task ExplainFileFilters()
    {
      var message = @"
This textbox allows you to filter your batch export down to files that match certain criteria.
Filters are delimited by a semicolon (;) and are case-insensitive. Wildcard (*) is not supported.\n
\n
Example Usage: masterchief;dervish\n
This will only export files with 'masterchief' and 'dervish' in their names.
";

      await ShowMessageModal(
        title: "File Filters",
        message: message );
    }

    private async Task ExplainTextureDefinitions()
    {
      var message = @"
Texture Definitions are text files that describe how materials use a particular texture.\n
Little is known about how these work in the engine, but they provide important information 
for setting up game-accurate shaders.";

      await ShowMessageModal(
        title: "Export Texture Definitions",
        message: message );
    }

    private async Task ExplainLODs()
    {
      var message = @"
This setting attempts to remove LOD (Level of Detail) meshes from the exported file. 
LOD Meshes are low-poly and are only used by the game engine when objects are far away.\n
\n
Due to meshes and nodes not following any specific naming convention, this option may not
always successfully remove LOD meshes from the exported file. This is expecially the case
for level geometry (maps).";

      await ShowMessageModal(
        title: "Remove LODs",
        message: message );
    }

    private async Task ExplainVolumes()
    {
      var message = @"
This setting attempts to remove trigger volumes and other invisible meshes from the exported file.
These are typically only present in level geometry (maps).\n
\n
Due to meshes and nodes not following any specific naming convention, this option may not
always successfully remove volume meshes from the exported file.";

      await ShowMessageModal(
        title: "Remove Volumes",
        message: message );
    }

    #endregion

  }

}
