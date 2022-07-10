using System;
using System.IO;
using System.Threading.Tasks;
using Assimp;
using H2AIndex.Common;
using H2AIndex.Common.Enumerations;
using H2AIndex.Models;
using Saber3D.Files;

namespace H2AIndex.Processes
{

  public class ExportModelProcess : ProcessBase
  {

    #region Data Members

    private IS3DFile _file;
    private Scene _scene;
    private ModelExportOptionsModel _modelOptions;
    private TextureExportOptionsModel _textureOptions;

    private string _outputPath;

    #endregion

    #region Constructor

    public ExportModelProcess( IS3DFile file, Tuple<ModelExportOptionsModel, TextureExportOptionsModel> options )
    {
      _file = file;
      _modelOptions = options.Item1.DeepCopy();
      _textureOptions = options.Item2.DeepCopy();
    }

    public ExportModelProcess( IS3DFile file, Scene scene, Tuple<ModelExportOptionsModel, TextureExportOptionsModel> options )
      : this( file, options )
    {
      _scene = scene;
    }

    #endregion

    #region Overrides

    protected override async Task OnInitializing()
    {
      _outputPath = GetOutputModelPath();
    }

    protected override async Task OnExecuting()
    {
      if ( _scene is null )
        await ConvertFileToAssimpScene();

      await WriteAssimpSceneToFile();

      if ( _modelOptions.ExportTextures )
        await ExportTextures();
    }

    #endregion

    #region Private Methods

    private string GetOutputModelPath()
    {
      var extension = _modelOptions.OutputFileFormat.GetFileExtension();
      var fName = Path.ChangeExtension( _file.Name, extension );

      return Path.Combine( _modelOptions.OutputPath, fName );
    }

    private async Task ConvertFileToAssimpScene()
    {
      var process = new ConvertModelToAssimpSceneProcess( _file );
      BindStatusToSubProcess( process );

      await process.Execute();
    }

    private async Task WriteAssimpSceneToFile()
    {
      try
      {
        Status = "Writing File";
        IsIndeterminate = true;

        var formatId = _modelOptions.OutputFileFormat.ToAssimpFormatId();
        using ( var context = new AssimpContext() )
        {
          context.ExportFile( _scene, _outputPath, formatId );
        }
      }
      catch ( Exception ex )
      {
        StatusList.AddError( _file.Name, "Failed to write the model file.", ex );
        throw;
      }
    }

    private async Task GatherTextures()
    {

    }

    private async Task ExportTextures()
    {

    }

    #endregion

  }

}
