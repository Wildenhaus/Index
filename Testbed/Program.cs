using System;
using System.IO;
using System.Linq;
using Aspose.ThreeD;
using Saber3D.Common;
using Saber3D.Files;
using Saber3D.Serializers;

namespace Testbed
{

  public static class Program
  {
    // Change this
    const string GAME_PATH = @"G:\Steam\steamapps\common\Halo The Master Chief Collection\";

    public static void Main( string[] args )
    {
      TrialException.SuppressTrialException = true;

      // Index all game files
      H2AFileContext.Global.OpenDirectory( GAME_PATH );

      TestReadTemplateModels();
      TestReadLevelGeometry();
      TestReadTextures();

      ExportArmatureTest( @"G:\h2a\re files\masterchief__h.tpl", @"F:\test.fbx" );
    }

    private static void ExportArmatureTest( string tplPath, string outFbxPath )
    {
      TrialException.SuppressTrialException = true;

      var tplStr = File.OpenRead( tplPath );
      var reader = new BinaryReader( tplStr );
      var tpl = new S3DTemplateSerializer().Deserialize( reader );
      var arm = new ArmatureExportTest( tpl.GeometryGraph );
      using ( var fs = File.Create( outFbxPath ) )
      {
        arm.Save( fs );
        fs.Flush();
      }
    }

    private static void ExportModelGeometry( string tplName, string outFbxPath )
    {
      var fileContext = H2AFileContext.Global;
      fileContext.OpenDirectory( GAME_PATH );

      var tplFile = fileContext.GetFiles( tplName )
        .Where( x => x.Name.Contains( tplName ) )
        .FirstOrDefault();

      if ( tplFile is null )
      {
        Console.WriteLine( "Nothing found for: {0}", tplName );
        return;
      }

      var stream = tplFile.GetStream();
      var outstream = ModelConverter.ConvertTpl( stream );
      using ( var outfile = File.Create( outFbxPath ) )
      {
        outstream.CopyTo( outfile );
        outfile.Flush();
      }
    }

    private static void ExportLevelGeometry( string lgName, string outFbxPath )
    {
      var fileContext = H2AFileContext.Global;
      var file = Directory.GetFiles( GAME_PATH, "*.pck", SearchOption.AllDirectories )
        .FirstOrDefault( x => x.Contains( Path.GetFileNameWithoutExtension( lgName ) ) );
      fileContext.OpenFile( file );

      var tplFile = fileContext.GetFiles( lgName )
        .Where( x => x.Name.Contains( lgName ) )
        .FirstOrDefault();

      if ( tplFile is null )
      {
        Console.WriteLine( "Nothing found for: {0}", lgName );
        return;
      }

      var stream = tplFile.GetStream();
      var outstream = ModelConverter.ConvertScn( stream );
      using ( var outfile = File.Create( outFbxPath ) )
      {
        outstream.CopyTo( outfile );
        outfile.Flush();
      }
    }

    private static void TestReadLevelGeometry()
    {
      var fileContext = H2AFileContext.Global;

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
      var fileContext = H2AFileContext.Global;

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
      var fileContext = H2AFileContext.Global;

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

  }

}