using System.IO;

namespace Saber3D.Files.FileTypes
{

  [FileSignature( "1SERlg" )]
  [FileExtension( ".lg" )]
  public class LgFile : S3DFile
  {

    #region Properties

    public override string FileTypeDisplay => "Level Geometry (.lg)";

    #endregion

    #region Constructor

    public LgFile( string name, Stream stream, IS3DFile parent = null )
      : base( name, stream, parent )
    {
    }

    #endregion

  }

}
