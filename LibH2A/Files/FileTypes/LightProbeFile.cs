using System.IO;

namespace Saber3D.Files.FileTypes
{

  [FileSignature( "1SERlight_probe" )]
  [FileExtension( ".light_probe2" )]
  public class LightProbeFile : S3DFile
  {

    #region Properties

    public override string FileTypeDisplay => "Light Probe (.light_probe2)";

    #endregion

    #region Constructor

    public LightProbeFile( string name, Stream stream, IS3DFile parent = null )
      : base( name, stream, parent )
    {
    }

    #endregion

  }

}
