using System;
using System.IO;
using System.Linq;
using Assimp;
using Saber3D.Common;
using Saber3D.Files;
using Saber3D.Serializers;

namespace Testbed
{

  public static class Program
  {
    // Change this
    static readonly IH2AFileContext FileContext = new H2AFileContext();
    const string GAME_PATH = @"G:\Steam\steamapps\common\Halo The Master Chief Collection\";

    public static void Main( string[] args )
    {
      // Index all game files
      FileContext.OpenDirectory( GAME_PATH );

      TestReadTemplateModels();
      TestReadLevelGeometry();
      TestReadTextures();
      TestConvertTexturesToDDS();
      TestConvertTexturesToTGA();

      ExportModelGeometry( "masterchief__h.tpl", @"F:\test.fbx" );
      ExportLevelGeometry( "newmombasa.lg", @"F:\testLG.fbx" );
    }

    private static void ExportModelGeometry( string tplName, string outFbxPath )
    {
      var fileContext = FileContext;

      var tplFile = fileContext.GetFiles( tplName )
        .Where( x => x.Name.Contains( tplName ) )
        .FirstOrDefault();

      if ( tplFile is null )
      {
        Console.WriteLine( "Nothing found for: {0}", tplName );
        return;
      }

      var stream = tplFile.GetStream();
      var reader = new BinaryReader( stream );
      var tpl = new S3DTemplateSerializer().Deserialize( reader );

      var scene = SceneExporter.CreateScene( tpl.Name, tpl.GeometryGraph, reader );
      using ( var ctx = new AssimpContext() )
      {
        var types = ctx.GetSupportedExportFormats();
        ctx.ExportFile( scene, outFbxPath, "fbx" );
      }
    }

    private static void ExportLevelGeometry( string lgName, string outFbxPath )
    {
      var fileContext = FileContext;

      var lgFile = fileContext.GetFiles( lgName )
        .Where( x => x.Name.Contains( lgName ) )
        .FirstOrDefault();

      if ( lgFile is null )
      {
        Console.WriteLine( "Nothing found for: {0}", lgName );
        return;
      }

      var stream = lgFile.GetStream();
      var reader = new BinaryReader( stream );
      var lg = new S3DSceneSerializer().Deserialize( reader );

      var scene = SceneExporter.CreateScene( lgFile.Name, lg.GeometryGraph, reader );
      using ( var ctx = new AssimpContext() )
      {
        var types = ctx.GetSupportedExportFormats();
        ctx.ExportFile( scene, outFbxPath, "fbx" );
      }
    }

    private static void TestReadLevelGeometry()
    {
      var fileContext = FileContext;

      var count = 0;
      var success = 0;

      foreach ( var file in fileContext.GetFiles( ".lg" ) )
      {
        try
        {
          count++;
          Console.WriteLine( file.Name );

          var stream = file.GetStream().ToBufferedStream();
          var reader = new BinaryReader( stream );
          var scn = new S3DSceneSerializer().Deserialize( reader );

          success++;
          Console.Title = $"{success}/{count}";
        }
        catch ( Exception ex )
        {
          Console.WriteLine( "  Failed to read {0}", file.Name );
          Console.WriteLine( "    {0}", ex.Message );
        }
      }

      Console.WriteLine( $"{success}/{count}" );
    }

    private static void TestReadTemplateModels()
    {
      var fileContext = FileContext;

      var count = 0;
      var success = 0;

      foreach ( var file in fileContext.GetFiles( ".tpl" ) )
      {
        try
        {
          count++;
          Console.WriteLine( file.Name );

          var stream = file.GetStream().ToBufferedStream();
          var reader = new BinaryReader( stream );
          new S3DTemplateSerializer().Deserialize( reader );

          success++;
          Console.Title = $"{success}/{count}";
        }
        catch ( Exception ex )
        {
          Console.WriteLine( "  Failed to read {0}", file.Name );
          Console.WriteLine( "    {0}", ex.Message );
        }
      }

      Console.WriteLine( $"{success}/{count}" );
    }

    private static void TestReadTextures()
    {
      var fileContext = FileContext;

      var count = 0;
      var success = 0;

      foreach ( var file in fileContext.GetFiles( ".pct" ) )
      {
        try
        {
          count++;
          Console.WriteLine( file.Name );

          var stream = file.GetStream();
          var reader = new BinaryReader( stream );
          new S3DPictureSerializer().Deserialize( reader );

          success++;
          Console.Title = $"{success}/{count}";
        }
        catch ( Exception ex )
        {
          Console.WriteLine( "  Failed to read {0}", file.Name );
          Console.WriteLine( "    {0}", ex.Message );
        }
      }

      Console.WriteLine( $"{success}/{count}" );
    }

    private static void TestConvertTexturesToDDS()
    {
      int success = 0, count = 0;
      foreach ( var file in FileContext.GetFiles( ".pct" ) )
      {
        var fs = file.GetStream();
        var reader = new BinaryReader( fs );
        var pct = new S3DPictureSerializer().Deserialize( reader );

        Console.ForegroundColor = ConsoleColor.White;
        Console.Write( "{0}...", file.Name );

        try
        {
          var converted = TextureConverter.ConvertToDDS( pct );
          if ( converted is null || converted.Length <= 0 )
            throw new Exception();

          Console.ForegroundColor = ConsoleColor.Green;
          Console.WriteLine( "SUCCESS" );
          success++;
        }
        catch ( Exception ex )
        {
          Console.ForegroundColor = ConsoleColor.Red;
          Console.WriteLine( "FAILED" );
        }

        Console.Title = $"{success}/{++count}";
      }
      Console.ForegroundColor = ConsoleColor.White;
    }

    private static void TestConvertTexturesToTGA()
    {
      int success = 0, count = 0;
      foreach ( var file in FileContext.GetFiles( ".pct" ) )
      {
        var fs = file.GetStream();
        var reader = new BinaryReader( fs );
        var pct = new S3DPictureSerializer().Deserialize( reader );

        Console.ForegroundColor = ConsoleColor.White;
        Console.Write( "{0}...", file.Name );

        try
        {
          var converted = TextureConverter.ConvertToTGA( pct, 0 );
          if ( converted is null || converted.Length <= 0 )
            throw new Exception();

          Console.ForegroundColor = ConsoleColor.Green;
          Console.WriteLine( "SUCCESS" );
          success++;
        }
        catch ( Exception ex )
        {
          Console.ForegroundColor = ConsoleColor.Red;
          Console.WriteLine( "FAILED" );
        }

        Console.Title = $"{success}/{++count}";
      }
      Console.ForegroundColor = ConsoleColor.White;
    }

  }

}