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

    public LightProbeFile( string name, H2AStream baseStream,
      long dataStartOffset, long dataEndOffset,
      IS3DFile parent = null )
      : base( name, baseStream, dataStartOffset, dataEndOffset, parent )
    {
    }

    #endregion

  }

}
