using System.IO;

namespace Saber3D.Files.FileTypes
{

  [FileSignature( "1SERsm_tpl_tbl" )]
  [FileExtension( ".tbl" )]
  public class TplTableFile : S3DFile
  {

    #region Properties

    public override string FileTypeDisplay => "Template Table (.tbl)";

    #endregion

    #region Constructor

    public TplTableFile( string name, Stream stream, IS3DFile parent = null )
      : base( name, stream, parent )
    {
    }

    #endregion

  }

}
