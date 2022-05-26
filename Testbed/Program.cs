using System;
using System.IO;
using System.Linq;
using Saber3D.Serializers;

namespace Saber3D
{

  public static class Program
  {

    public static void Main( string[] args )
    {
      var files = new string[]
      {
        //@"G:\h2a\re files\banshee__h.tpl",
        //@"G:\h2a\re files\dervish__h.tpl",
        @"G:\h2a\d\shared\_database_\bldgbottom_bulletingscreen__screen_act.tpl",
      };

      files = Directory.GetFiles( @"G:\h2a\d\shared\", "*.tpl", SearchOption.AllDirectories );

      var count = 0;
      var success = 0;
      foreach ( var filePath in files )
      {
        try
        {
          count++;
          Console.WriteLine( filePath );
          var file = File.OpenRead( filePath );
          //var stream = CreateOgmStreamSegment( file );
          var stream = CreateSerTplStreamSegment( file );
          stream.Position = FindDataOffset( stream, MAGIC_TPL1 );

          var reader = new BinaryReader( stream );

          var s = new S3DTemplateSerializer();
          var tpl = s.Deserialize( reader );
          success++;
          Console.Title = $"{success}/{count}";

          //if ( true )
          //{
          //  Console.WriteLine( tpl.Name );
          //  Console.WriteLine( "  {0,10}: {1}", "name", tpl.PropertyFlags[ 0 ] );
          //  Console.WriteLine( "  {0,10}: {1}", "nameClass", tpl.PropertyFlags[ 1 ] );
          //  Console.WriteLine( "  {0,10}: {1}", "state", tpl.PropertyFlags[ 2 ] );
          //  Console.WriteLine( "  {0,10}: {1}", "affixes", tpl.PropertyFlags[ 3 ] );
          //  Console.WriteLine( "  {0,10}: {1}", "ps", tpl.PropertyFlags[ 4 ] );
          //  Console.WriteLine( "  {0,10}: {1}", "skin", tpl.PropertyFlags[ 5 ] );
          //  Console.WriteLine( "  {0,10}: {1}", "trackAnim", tpl.PropertyFlags[ 6 ] );
          //  Console.WriteLine( "  {0,10}: {1}", "OnReadAnimExtra", tpl.PropertyFlags[ 7 ] );
          //  Console.WriteLine( "  {0,10}: {1}", "bbox", tpl.PropertyFlags[ 8 ] );
          //  Console.WriteLine( "  {0,10}: {1}", "lodDef", tpl.PropertyFlags[ 9 ] );
          //  Console.WriteLine( "  {0,10}: {1}", "TexList", tpl.PropertyFlags[ 10 ] );
          //  Console.WriteLine( "  {0,10}: {1}", "geomMng", tpl.PropertyFlags[ 11 ] );
          //  Console.WriteLine( "  {0,10}: {1}", "externData", tpl.PropertyFlags[ 12 ] );
          //  //Console.WriteLine( "NameClass: {0}", tpl.NameClass );
          //  //Console.WriteLine( "State: {0}", tpl.Unk_State );
          //  //Console.WriteLine( "Affixes: {0}", tpl.Affixes );
          //  //Console.WriteLine( "Ps: {0}", tpl.Unk_Ps );
          //}
        }
        catch ( Exception ex )
        {
          Console.WriteLine( "Failed to read {0}", filePath );
          Console.WriteLine( ex.Message );
          //return;
        }
      }
    }

    private static readonly byte[] MAGIC_1SERtpl = new byte[]
        {
      0x31, 0x53, 0x45, 0x52, 0x74, 0x70, 0x6C, 0x00
        };

    private static readonly byte[] MAGIC_TPL1 = new byte[]
    {
      0x54, 0x50, 0x4C, 0x31
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

    private static Stream CreateTplStreamSegment( Stream stream )
    {
      var memoryStream = new MemoryStream();
      stream.Position = FindDataOffset( stream, MAGIC_TPL1 );
      stream.CopyTo( memoryStream );

      memoryStream.Position = 0;
      return memoryStream;
    }

    private static Stream CreateOgmStreamSegment( Stream stream )
    {
      var memoryStream = new MemoryStream();
      stream.Position = FindDataOffset( stream, MAGIC_OGM1 );
      stream.CopyTo( memoryStream );

      memoryStream.Position = 0;
      return memoryStream;
    }

    private static Stream CreateSerTplStreamSegment( Stream stream )
    {
      var memoryStream = new MemoryStream();
      stream.Position = FindDataOffset( stream, MAGIC_1SERtpl );
      stream.CopyTo( memoryStream );

      memoryStream.Position = 0;
      return memoryStream;
    }

  }
}