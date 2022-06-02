using System;
using System.IO;
using System.Linq;
using Saber3D.Serializers;

namespace Testbed
{

  public static class Program
  {

    // Change this
    const string DATA_PATH = @"G:\h2a\d\";

    public static void Main( string[] args )
    {
      ReadTpls();
      ReadScns();

      //ExportModelGeometry( @"G:\h2a\d\shared\_database_\dervish__h.tpl", @"F:\test.fbx" );
      //ExportLevelGeometry( @"G:\h2a\d\03b_newmombasa\_scene_\03b_newmombasa.lg", @"F:\test.fbx" );
    }

    private static void ExportModelGeometry( string tplPath, string outFbxPath )
    {
      var file = File.OpenRead( tplPath );
      var stream = CreateSerTplStreamSegment( file );
      stream.Position = FindDataOffset( stream, MAGIC_TPL1 );

      var outstream = FbxConverter.ConvertTpl( stream );
      using ( var outfile = File.Create( outFbxPath ) )
      {
        outstream.CopyTo( outfile );
        outfile.Flush();
      }
    }

    //private static void ExportLevelGeometry( string lgPath, string outFbxPath )
    //{
    //  var file = File.OpenRead( lgPath );
    //  var stream = CreateSerLgStreamSegment( file );
    //  stream.Position = FindDataOffset( stream, MAGIC_SCN1 );

    //  var outstream = FbxConverter.ConvertScn( stream );
    //  using ( var outfile = File.Create( outFbxPath ) )
    //  {
    //    outstream.CopyTo( outfile );
    //    outfile.Flush();
    //  }
    //}

    private static void ReadScns()
    {
      var files = Directory.GetFiles( DATA_PATH, "*.lg", SearchOption.AllDirectories );

      var count = 0;
      var success = 0;
      foreach ( var filePath in files )
      {
        try
        {
          count++;
          Console.WriteLine( filePath );
          var file = File.OpenRead( filePath );
          var stream = CreateSerLgStreamSegment( file );
          stream.Position = FindDataOffset( stream, MAGIC_SCN1 );

          var reader = new BinaryReader( stream );
          var scn = new S3DSceneSerializer().Deserialize( reader );

          success++;
          Console.Title = $"{success}/{count}";
        }
        catch ( Exception ex )
        {
          Console.WriteLine( "Failed to read {0}", filePath );
          Console.WriteLine( ex.Message );
        }
      }
      Console.WriteLine( $"{success}/{count}" );

    }

    private static void ReadTpls()
    {
      var files = Directory.GetFiles( DATA_PATH, "*.tpl", SearchOption.AllDirectories );

      var count = 0;
      var success = 0;

      foreach ( var filePath in files )
      {
        //try
        //{


        if ( filePath.Contains( "cannon__h.tpl" ) )
          continue;
        if ( filePath.Contains( "scorpion__h.tpl" ) )
          continue;

        count++;
        Console.WriteLine( filePath );

        var file = File.OpenRead( filePath );
        var stream = CreateSerTplStreamSegment( file );
        stream.Position = FindDataOffset( stream, MAGIC_TPL1 );

        //FbxConverter.ConvertTpl( stream );
        var reader = new BinaryReader( stream );
        new S3DTemplateSerializer().Deserialize( reader );

        success++;
        Console.Title = $"{success}/{count}";
        //}
        //catch ( Exception ex )
        //{
        //  Console.WriteLine( filePath );
        //  //Console.WriteLine( "  Failed to read {0}", filePath );
        //  Console.WriteLine( "    {0}", ex.Message );
        //}
      }
      Console.WriteLine( $"{success}/{count}" );
    }

    private static readonly byte[] MAGIC_1SERtpl = new byte[]
        {
      0x31, 0x53, 0x45, 0x52, 0x74, 0x70, 0x6C, 0x00
        };

    private static readonly byte[] MAGIC_1SERlg = new byte[]
        {
      0x31, 0x53, 0x45, 0x52, 0x6C, 0x67
        };

    private static readonly byte[] MAGIC_TPL1 = new byte[]
    {
      0x54, 0x50, 0x4C, 0x31
    };

    private static readonly byte[] MAGIC_SCN1 = new byte[]
    {
      0x53, 0x43, 0x4E, 0x31
    };

    private static readonly byte[] MAGIC_OGM1 = new byte[]
    {
      0x4F, 0x47, 0x4D, 0x31
    };

    private static long FindDataOffset( Stream stream, byte[] data )
    {
      stream.Position = 0;
      long found = -1;
      int curr;
      while ( ( curr = stream.ReadByte() ) > -1 )
      {
        if ( curr == data[ 0 ] )
        {
          stream.Position--;
          byte[] buffer = new byte[ data.Length ];
          stream.Read( buffer, 0, data.Length );
          if ( buffer.SequenceEqual( data ) )
          {
            found = stream.Position - data.Length;
            break;
          }
          else
            stream.Position -= data.Length - 1;
        }
      }

      return found;
    }

    private static Stream CreateSerTplStreamSegment( Stream stream )
    {
      var memoryStream = new MemoryStream();
      stream.Position = FindDataOffset( stream, MAGIC_1SERtpl );
      stream.CopyTo( memoryStream );

      memoryStream.Position = 0;
      return memoryStream;
    }

    private static Stream CreateSerLgStreamSegment( Stream stream )
    {
      var memoryStream = new MemoryStream();
      stream.Position = FindDataOffset( stream, MAGIC_1SERlg );
      stream.CopyTo( memoryStream );

      memoryStream.Position = 0;
      return memoryStream;
    }

  }
}