using System;
using System.IO;
using System.Linq;
using Aspose.ThreeD;
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
      //TestReadTemplateModels();
      //TestReadLevelGeometry();

      ExportArmatureTest( @"G:\h2a\re files\masterchief__h.tpl", @"F:\test.fbx" );

      //ExportModelGeometry( @"dervish__h.tpl", @"F:\dervish.fbx" );
      //ExportLevelGeometry( @"newmombasa.lg", @"F:\test.fbx" );
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
      fileContext.OpenDirectory( GAME_PATH );

      var count = 0;
      var success = 0;

      foreach ( var file in fileContext.GetFiles( ".lg" ) )
      {
        try
        {
          count++;
          Console.WriteLine( file.Name );

          var stream = file.GetStream();
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
      fileContext.OpenDirectory( GAME_PATH );

      var count = 0;
      var success = 0;

      foreach ( var file in fileContext.GetFiles( ".tpl" ) )
      {
        try
        {
          count++;
          Console.WriteLine( file.Name );

          var stream = file.GetStream();
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

  }

  //static class FlagAnalyzer
  //{

  //  static Dictionary<S3DGeometryBufferFlags, FlagInfo> Info
  //    = CreateInfoDict();

  //  public static void Analyze()
  //  {
  //    var analyzed = 0;
  //    foreach ( var file in GetFiles() )
  //    {
  //      AnalyzeFile( file );
  //      Console.Title = $"{++analyzed} | {file}";
  //    }

  //    PrintResults();
  //  }

  //  static void AnalyzeFile( string filePath )
  //  {
  //    var graph = GetGeometryGraph( filePath );
  //    if ( graph is null )
  //      return;

  //    foreach ( var buffer in graph.Buffers )
  //    {
  //      var bufferFlags = buffer.Flags;
  //      foreach ( var flag in EnumerateFlags( bufferFlags ) )
  //      {
  //        var flagInfo = Info[ flag ];

  //        flagInfo.Occurences++;
  //        flagInfo.Sizes.Add( buffer.ElementSize );
  //        flagInfo.UsedIn.Add( bufferFlags );
  //        flagInfo.Files.Add( Path.GetFileName( filePath ) );
  //      }
  //    }
  //  }

  //  static void PrintResults()
  //  {
  //    foreach ( var flag in Enum.GetValues<S3DGeometryBufferFlags>() )
  //    {
  //      var flagInfo = Info[ flag ];
  //      Console.WriteLine( flag );
  //      Console.WriteLine( "  - Occurences: {0}", flagInfo.Occurences );
  //      Console.WriteLine( "  - Exclusivity: {0}", flagInfo.Exclusivity );
  //      Console.WriteLine( "  - Sizes: " );
  //      foreach ( var size in flagInfo.Sizes.OrderBy( x => x ) )
  //        Console.WriteLine( "    - 0x{0:X}", size );
  //      Console.WriteLine( "  - Instances: " );
  //      foreach ( var instance in flagInfo.UsedIn.OrderBy( x => x ) )
  //        Console.WriteLine( "    - {0}", instance );
  //      Console.WriteLine( "  - Files: " );
  //      foreach ( var file in flagInfo.Files.Take( 10 ) )
  //        Console.WriteLine( "    - {0}", file );

  //      Console.WriteLine();
  //      Console.WriteLine( new String( '-', 40 ) );
  //      Console.WriteLine();
  //    }
  //  }

  //  static S3DGeometryGraph GetGeometryGraph( string filePath )
  //  {
  //    try
  //    {
  //      if ( Path.GetExtension( filePath ) == ".tpl" )
  //      {
  //        var file = File.OpenRead( filePath );
  //        var stream = Program.CreateSerTplStreamSegment( file );
  //        stream.Position = Program.FindDataOffset( stream, Program.MAGIC_TPL1 );

  //        //FbxConverter.ConvertTpl( stream );
  //        var reader = new BinaryReader( stream );
  //        var tpl = new S3DTemplateSerializer().Deserialize( reader );
  //        return tpl.GeometryGraph;
  //      }
  //      else
  //      {
  //        var file = File.OpenRead( filePath );
  //        var stream = Program.CreateSerLgStreamSegment( file );
  //        stream.Position = Program.FindDataOffset( stream, Program.MAGIC_SCN1 );

  //        //FbxConverter.ConvertTpl( stream );
  //        var reader = new BinaryReader( stream );
  //        var scn = new S3DSceneSerializer().Deserialize( reader );
  //        return scn.GeometryGraph;
  //      }
  //    }
  //    catch ( Exception ex )
  //    {
  //      Console.WriteLine( ex.Message );
  //      return null;
  //    }

  //  }

  //  static IEnumerable<string> GetFiles()
  //  {
  //    const string DATA_PATH = @"G:\h2a\d\";
  //    foreach ( var f in Directory.GetFiles( DATA_PATH, "*.tpl", SearchOption.AllDirectories ) )
  //      yield return f;

  //    foreach ( var f in Directory.GetFiles( DATA_PATH, "*.lg", SearchOption.AllDirectories ) )
  //      yield return f;
  //  }

  //  static Dictionary<S3DGeometryBufferFlags, FlagInfo> CreateInfoDict()
  //  {
  //    var dict = new Dictionary<S3DGeometryBufferFlags, FlagInfo>();
  //    foreach ( var flag in Enum.GetValues<S3DGeometryBufferFlags>() )
  //      dict[ flag ] = new FlagInfo( flag );
  //    return dict;
  //  }

  //  static IEnumerable<S3DGeometryBufferFlags> EnumerateFlags( S3DGeometryBufferFlags flags )
  //  {
  //    foreach ( var flag in Enum.GetValues<S3DGeometryBufferFlags>() )
  //      if ( flags.HasFlag( flag ) )
  //        yield return flag;
  //  }

  //}

  //class FlagInfo
  //{
  //  public S3DGeometryBufferFlags Flag;
  //  public int Occurences;
  //  public HashSet<S3DGeometryBufferFlags> UsedIn = new HashSet<S3DGeometryBufferFlags>();
  //  public HashSet<ushort> Sizes = new HashSet<ushort>();
  //  public HashSet<string> Files = new HashSet<string>();

  //  public string Exclusivity
  //  {
  //    get
  //    {
  //      if ( Files.Count == 0 )
  //        return "Unused";

  //      if ( Files.All( x => x.Contains( ".lg", StringComparison.InvariantCultureIgnoreCase ) ) )
  //        return "Level Geometry";
  //      if ( Files.All( x => x.Contains( "grass", StringComparison.InvariantCultureIgnoreCase ) ) )
  //        return "Grass";
  //      if ( Files.All( x => x.Contains( ".tpl", StringComparison.InvariantCultureIgnoreCase ) ) )
  //        return "TPL";

  //      return "None";
  //    }
  //  }

  //  public FlagInfo( S3DGeometryBufferFlags flag )
  //  {
  //    Flag = flag;
  //  }
  //}

}