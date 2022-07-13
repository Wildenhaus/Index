namespace Saber3D.Files.FileTypes
{

  [FileSignature( "1SERssl_data" )]
  [FileExtension( ".ssl_data" )]
  public class SslDataFile : S3DFile
  {

    #region Properties

    public override string FileTypeDisplay => "Scripting Data (.ssl_data)";

    #endregion

    #region Constructor

    public SslDataFile( string name, H2AStream baseStream,
      long dataStartOffset, long dataEndOffset,
      IS3DFile parent = null )
      : base( name, baseStream, dataStartOffset, dataEndOffset, parent )
    {
    }

    #endregion

  }

}
