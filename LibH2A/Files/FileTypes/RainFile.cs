using System.IO;

namespace Saber3D.Files.FileTypes
{

  [FileSignature( "1SERrain" )]
  [FileExtension( ".rain" )]
  public class RainFile : S3DFile
  {

    #region Properties

    public override string FileTypeDisplay => "Rain (.rain)";

    #endregion

    #region Constructor

    public RainFile( string name, Stream stream, IS3DFile parent = null )
      : base( name, stream, parent )
    {
    }

    #endregion

  }

}
