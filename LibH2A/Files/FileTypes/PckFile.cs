using System.IO;

namespace Saber3D.Files
{

  [FileSignature( "1SERpak" )]
  [FileExtension( ".pck" )]
  public class PckFile : S3DContainerFile
  {

    #region Properties

    public override string FileTypeDisplay => "Pack File (.pck)";

    #endregion

    #region Constructor

    public PckFile( string name, Stream stream, IS3DFile parent = null )
      : base( name, stream, parent )
    {
    }

    public static PckFile FromStream( string name, H2ADecompressionStream stream, IS3DFile parent = null )
    {
      var file = new PckFile( name, stream, parent );
      file.Initialize();

      return file;
    }

    public static PckFile FromFile( string filePath )
    {
      var name = Path.GetFileName( filePath );
      return FromStream( name, H2ADecompressionStream.FromFile( filePath ) );
    }

    #endregion

  }

}
