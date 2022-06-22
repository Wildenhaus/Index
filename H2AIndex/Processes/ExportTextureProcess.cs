using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using H2AIndex.Common;
using H2AIndex.Common.Enumerations;
using H2AIndex.Models;
using H2AIndex.Services;
using ImageMagick;
using Microsoft.Extensions.DependencyInjection;
using Saber3D.Data.Textures;
using Saber3D.Files;
using Saber3D.Files.FileTypes;
using Saber3D.Serializers.Configurations;

namespace H2AIndex.Processes
{

  public class ExportTextureProcess : ProcessBase
  {

    // TODO: This entire class is fucking gross.

    #region Data Members

    private readonly IH2AFileContext _fileContext;
    private readonly ITextureConversionService _textureConversionService;

    private readonly PictureFile _file;
    private readonly TextureExportOptionsModel _options;

    private string _outputFilePath;

    private S3DPicture _picture;
    private TextureModel _texture;
    private MagickImageCollection _imageCollection;

    private List<TextureImage> _targetImages;

    #endregion

    #region Properties

    public string TextureName
    {
      get => Path.GetFileNameWithoutExtension( _file.Name );
    }

    #endregion

    #region Constructor

    public ExportTextureProcess( PictureFile file, TextureExportOptionsModel options )
    {
      _fileContext = ServiceProvider.GetRequiredService<IH2AFileContext>();
      _textureConversionService = ServiceProvider.GetRequiredService<ITextureConversionService>();

      _file = file;
      _options = options.DeepCopy();
    }

    #endregion

    #region Overrides

    protected override async Task OnExecuting()
    {
      _picture = _file.Deserialize();

      if ( !_options.OverwriteExisting && CheckIfFileExists() )
      {
        StatusList.AddWarning( _outputFilePath,
          "File already exists and the 'Overwrite Existing' option is disabled. Skipping." );

        return;
      }

      if ( _options.OutputFileFormat != TextureFileFormat.DDS )
      {

        try
        {
          await LoadTexture();

          if ( IsTextureNormalMap() )
            await ProcessNormalMaps();

          await WriteOutputFiles();
        }
        finally
        {
          _texture?.Dispose();
          _imageCollection?.Dispose();
        }
      }
      else
        await ExportRawDds();

      if ( _options.ExportTextureDefinition )
        await ExportTextureDefinition();
    }

    protected override async Task OnComplete()
    {
      _texture?.Dispose();
      _imageCollection?.Dispose();

      _picture = null;
      _texture = null;
      _imageCollection = null;
      _targetImages = null;
    }

    #endregion

    #region Private Methods

    private string GetOutputFilePath( int? faceIndex = null, int? mipIndex = null )
    {
      var fName = Path.GetFileNameWithoutExtension( _file.Name );

      if ( _options.OutputFileFormat != TextureFileFormat.DDS )
      {
        if ( faceIndex.HasValue )
          fName += $"_{faceIndex.Value}";
        if ( mipIndex.HasValue )
          fName += $"_{mipIndex.Value}";
      }

      fName += $".{_options.OutputFileFormat.ToString().ToLower()}";

      var outputFilePath = Path.Combine( _options.OutputPath, fName );
      return outputFilePath;
    }

    private bool CheckIfFileExists()
    {
      var fName = GetOutputFilePath(
        faceIndex: _picture.Faces > 1 ? 0 : null,
        mipIndex: _options.ExportAllMips ? 0 : null );

      return File.Exists( fName );
    }

    private bool IsTextureNormalMap()
    {
      var baseFileName = Path.GetFileNameWithoutExtension( _file.Name );

      if ( baseFileName.EndsWith( "_nm" ) )
        return true;

      if ( baseFileName.EndsWith( "_det" ) )
        return true;

      return false;
    }

    private async Task LoadTexture()
    {
      IsIndeterminate = true;
      Status = "Loading Texture";

      try
      {
        _texture = await _textureConversionService.LoadTexture( _file );

        _targetImages = new List<TextureImage>();
        var targetImageIndices = new HashSet<(int? face, int? mip, int index)>();
        for ( var faceIndex = 0; faceIndex < _texture.FaceCount; faceIndex++ )
        {
          var face = _texture.Faces[ faceIndex ];

          if ( _options.ExportAllMips )
            foreach ( var mip in face.MipMaps )
              targetImageIndices.Add( (_texture.IsCubeMap ? faceIndex : null, mip.MipLevel, mip.ImageIndex) );
          else
            targetImageIndices.Add( (_texture.IsCubeMap ? faceIndex : null, null, face.MipMaps.First().ImageIndex) );
        }

        var indices = targetImageIndices.Select( x => x.index );
        _imageCollection = await _textureConversionService.CreateMagickImageCollection( _texture.DdsImage, indices );
        foreach ( var pair in targetImageIndices )
        {
          _targetImages.Add( new TextureImage
          {
            FaceIndex = pair.face,
            MipIndex = pair.mip,
            Image = _imageCollection[ pair.index ]
          } );
        }
      }
      catch ( Exception ex )
      {
        StatusList.AddError( _file.Name, "Failed to load texture", ex );
        throw;
      }
    }

    private async Task ProcessNormalMaps()
    {
      Status = _targetImages.Count > 1 ? "Processing Normal Maps" : "Processing Normal Map";

      try
      {
        foreach ( var targetImage in _targetImages )
        {
          if ( _options.OutputNormalMapFormat == NormalMapFormat.OpenGL )
            await _textureConversionService.InvertGreenChannel( targetImage.Image );

          if ( _options.RecalculateNormalMapZChannel )
            await _textureConversionService.RecalculateZChannel( targetImage.Image );
        }
      }
      catch ( Exception ex )
      {
        StatusList.AddError( _file.Name, "Failed to process one or more normal maps.", ex );
        throw;
      }
    }

    private async Task WriteOutputFiles()
    {
      Status = _targetImages.Count > 1 ? "Writing Files" : "Writing File";

      foreach ( var targetImage in _targetImages )
        WriteOutputFile( targetImage );
    }

    private async Task WriteOutputFile( TextureImage targetImage )
    {
      try
      {
        var filePath = GetOutputFilePath( targetImage.FaceIndex, targetImage.MipIndex );

        using ( var outStream = File.Create( filePath ) )
        {
          // TODO: Was getting a null ref (already disposed) when using WriteAsync for some output file types
          targetImage.Image.Write( outStream, GetMagickOutputFormat( _options.OutputFileFormat ) );
          await outStream.FlushAsync();
        }
      }
      catch ( Exception ex )
      {
        StatusList.AddError( _file.Name, "Failed to write output file.", ex );
        throw;
      }
    }

    private async Task ExportRawDds()
    {
      Status = "Loading Texture";
      using ( var ddsStream = await _textureConversionService.GetDDSStream( _file ) )
      {
        Status = "Writing File";
        using ( var outFile = File.Create( GetOutputFilePath() ) )
        {
          await ddsStream.CopyToAsync( outFile );
          await outFile.FlushAsync();
        }
      }
    }

    private async Task ExportTextureDefinition()
    {
      var tdFileName = Path.ChangeExtension( _file.Name, ".td" );
      var tdFile = _fileContext.GetFile<TextureDefinitionFile>( tdFileName );
      if ( tdFile is null )
      {
        StatusList.AddWarning( tdFileName, "Could not find Texture Definition. It isn't loaded or doesn't exist." );
        return;
      }

      try
      {
        var stream = tdFile.GetStream();
        var serializer = new FileScriptingSerializer<TextureDefinition>();
        var textureDef = serializer.Deserialize( stream );

        var outputFileName = Path.ChangeExtension( tdFile.Name, "json" );
        var outputPath = Path.Combine( _options.OutputPath, outputFileName );

        using var fs = File.Create( outputPath );

        var jsonOptions = new JsonSerializerOptions
        {
          WriteIndented = true,
          DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        await JsonSerializer.SerializeAsync( fs, textureDef, jsonOptions );

        await fs.FlushAsync();
      }
      catch ( Exception ex )
      {
        StatusList.AddError( tdFileName, "Encountered an error attempting to export the texture definition.", ex );
      }
    }

    private MagickFormat GetMagickOutputFormat( TextureFileFormat format )
    {
      switch ( format )
      {
        case TextureFileFormat.TGA:
          return MagickFormat.Tga;
        case TextureFileFormat.EXR:
          return MagickFormat.Exr;
        case TextureFileFormat.JPEG:
          return MagickFormat.Jpeg;
        case TextureFileFormat.PNG:
          return MagickFormat.Png;
        case TextureFileFormat.QOI:
          return MagickFormat.Qoi;
        default:
          throw new Exception( $"Unsupported format: {format}" );
      }
    }

    #endregion

    #region Embedded Types

    private class TextureImage
    {

      public IMagickImage<float> Image;
      public int? FaceIndex;
      public int? MipIndex;

    }

    #endregion

  }

}
