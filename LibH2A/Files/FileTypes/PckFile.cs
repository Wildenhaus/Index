namespace Saber3D.Files
{

  [FileSignature( "1SERpak" )]
  [FileExtension( ".pck" )]
  public class PckFile : S3DContainerFile
  {

    #region Properties

    public override string FileTypeDisplay => "Pack File (.pck)";

    #endregion

    #region Constructor

    public PckFile( string name, H2AStream baseStream,
      long dataStartOffset, long dataEndOffset,
      IS3DFile parent = null )
      : base( name, baseStream, dataStartOffset, dataEndOffset, parent )
    {
    }

    #endregion

  }

}
