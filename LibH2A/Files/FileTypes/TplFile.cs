using System.IO;

namespace Saber3D.Files.FileTypes
{

  [FileSignature( "1SERtpl" )]
  [FileExtension( ".tpl" )]
  public class TplFile : S3DFile
  {

    #region Properties

    public override string FileTypeDisplay => "Model/Template (.tpl)";

    #endregion

    #region Constructor

    public TplFile( string name, Stream stream, IS3DFile parent = null )
      : base( name, stream, parent )
    {
    }

    #endregion

  }

}