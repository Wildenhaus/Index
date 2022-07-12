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
    const string GAME_PATH = @"G:\Steam\steamapps\common\Halo The Master Chief Collection\";

    public static void Main( string[] args )
    {
      // Index all game files
      //H2AFileContext.Global.OpenDirectory( GAME_PATH );

      //TestReadTemplateModels();
      //TestReadLevelGeometry();
      //TestReadTextures();
      //TestConvertTexturesToDDS();
      //TestConvertTexturesToTGA();

      //ExportModelGeometry( "masterchief__h.tpl", @"F:\test.fbx" );
      //ExportLevelGeometry( "newmombasa.lg", @"F:\testLG.fbx" );

      //H2AFileContext.Global.OpenFile( @"F:\floodcombat_elite__h.tpl" );
      //var file = H2AFileContext.Global.GetFile( "floodcombat_elite__h.tpl" );
      //var tpl = new S3DTemplateSerializer().Deserialize( new BinaryReader( file.GetStream() ) );

      //foreach ( var obj in tpl.GeometryGraph.Objects )
      //  Console.WriteLine( obj.UnkName );

      H2AFileContext.Global.OpenDirectory( GAME_PATH );
      var file = H2AFileContext.Global.GetFile( "warthog__h.tpl" );
      using ( var fs = File.Create( @"F:\warthog__h.tpl" ) )
      {
        var s = file.GetStream();
        s.CopyTo( fs );
        fs.Flush();
      }

      //LoadFbx( @"Z:\Blender\Models\Destiny 2\Enemies\Fallen Marauder\Marauder.fbx" );
      //LoadFbx( @"G:\h2a\test\dervish__h.fbx" );
    }


    private static void LoadFbx( string path )
    {
      Scene scene;
      using ( var ctx = new AssimpContext() )
        scene = ctx.ImportFile( path );

      var rootNode = scene.RootNode;

      void Print( Node node, int level = 0 )
      {
        Console.WriteLine( "{0}{1}", new string( ' ', level ), node.Name );
        foreach ( var child in node.Children )
          Print( child, level + 1 );
      }
      Print( rootNode );
    }

    private static void ExportModelGeometry( string tplName, string outFbxPath )
    {
      var fileContext = H2AFileContext.Global;

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
      var fileContext = H2AFileContext.Global;

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

    private static void TestConvertTexturesToDDS()
    {
      int success = 0, count = 0;
      foreach ( var file in H2AFileContext.Global.GetFiles( ".pct" ) )
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
      foreach ( var file in H2AFileContext.Global.GetFiles( ".pct" ) )
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