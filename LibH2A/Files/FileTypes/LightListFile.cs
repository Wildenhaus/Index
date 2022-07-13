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

    public LightListFile( string name, H2AStream baseStream,
      long dataStartOffset, long dataEndOffset,
      IS3DFile parent = null )
      : base( name, baseStream, dataStartOffset, dataEndOffset, parent )
    {
    }

    #endregion

  }

}
