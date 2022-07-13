namespace Saber3D.Files.FileTypes
{

  [FileExtension( ".ssl" )]
  public class SslFile : S3DFile
  {

    #region Properties

    public override string FileTypeDisplay => "Script (.ssl)";

    #endregion

    #region Constructor

    public SslFile( string name, H2AStream baseStream,
      long dataStartOffset, long dataEndOffset,
      IS3DFile parent = null )
      : base( name, baseStream, dataStartOffset, dataEndOffset, parent )
    {
    }

    #endregion

  }

}
