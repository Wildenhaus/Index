using System;
using System.Linq;
using Assimp;

namespace H2AIndex.Common.Enumerations
{

  public enum ModelFileFormat
  {
    DAE,
    FBX,
    GLTF2,
    JSON,
    STL,
    X3D,
    XML
  }

  public static class ModelFileFormatExtensions
  {

    public static string ToAssimpFormatId( this ModelFileFormat format )
    {
      switch ( format )
      {
        case ModelFileFormat.DAE:
          return "collada";
        case ModelFileFormat.FBX:
          return "fbx";
        case ModelFileFormat.GLTF2:
          return "glb2";
        case ModelFileFormat.JSON:
          return "assjson";
        case ModelFileFormat.STL:
          return "stl";
        case ModelFileFormat.X3D:
          return "x3d";
        case ModelFileFormat.XML:
          return "assxml";
        default:
          throw new Exception( $"Unsupported format: {format}" );
      }
    }

    public static string GetFileExtension( this ModelFileFormat format )
    {
      var formatId = format.ToAssimpFormatId();

      using ( var context = new AssimpContext() )
      {
        var formats = context.GetSupportedExportFormats();
        var match = formats.First( c => c.FormatId == formatId );

        return match.FileExtension;
      }
    }

  }

}
