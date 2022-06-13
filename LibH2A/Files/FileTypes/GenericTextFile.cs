using System.IO;

namespace Saber3D.Files.FileTypes
{

  public class GenericTextFile : S3DFile
  {

    #region Properties

    public override string FileTypeDisplay => "Text File";

    #endregion

    #region Constructor

    public GenericTextFile( string name, Stream stream, IS3DFile parent = null )
      : base( name, stream, parent )
    {
    }

    #endregion

  }

}
