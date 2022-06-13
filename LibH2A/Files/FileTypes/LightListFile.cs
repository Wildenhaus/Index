using System.IO;

namespace Saber3D.Files.FileTypes
{

  [FileSignature( "1SERlight_list" )]
  [FileExtension( ".light_list" )]
  public class LightListFile : S3DFile
  {

    #region Properties

    public override string FileTypeDisplay => "Light List (.light_list)";

    #endregion

    #region Constructor

    public LightListFile( string name, Stream stream, IS3DFile parent = null )
      : base( name, stream, parent )
    {
    }

    #endregion

  }

}
