using System.IO;

namespace Saber3D.Files.FileTypes
{

  [FileExtension( ".pct" )]
  public class PictureFile : S3DFile
  {

    #region Properties

    public override string FileTypeDisplay => "Texture (.pct)";

    #endregion

    #region Constructor

    public PictureFile( string name, Stream stream, IS3DFile parent = null )
      : base( name, stream, parent )
    {
    }

    #endregion

  }

}
