using System.IO;

namespace Saber3D.Files.FileTypes
{

  [FileSignature( "1SERdom_list" )]
  [FileExtension( ".dom_list" )]
  public class DomListFile : S3DFile
  {

    #region Properties

    public override string FileTypeDisplay => "DOM List (.dom_list)";

    #endregion

    #region Constructor

    public DomListFile( string name, Stream stream, IS3DFile parent = null )
      : base( name, stream, parent )
    {
    }

    #endregion

  }

}
