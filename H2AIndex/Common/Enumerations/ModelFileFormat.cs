using System;

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
      using ( var ctx = new Assimp.AssimpContext() )
      {
        var f = ctx.GetSupportedExportFormats();
      }

      switch ( format )
      {
        case ModelFileFormat.DAE:
          return "collada";
        case ModelFileFormat.FBX:
          return "fbx";
        case ModelFileFormat.GLTF2:
          return "gtlf2";
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

  }

}
