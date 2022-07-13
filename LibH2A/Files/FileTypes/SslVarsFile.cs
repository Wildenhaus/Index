namespace Saber3D.Files.FileTypes
{

  [FileExtension( ".ssl_vars" )]
  public class SslVarsFile : S3DFile
  {

    #region Properties

    public override string FileTypeDisplay => "Scripting Variables (.ssl_vars)";

    #endregion

    #region Constructor

    public SslVarsFile( string name, H2AStream baseStream,
      long dataStartOffset, long dataEndOffset,
      IS3DFile parent = null )
      : base( name, baseStream, dataStartOffset, dataEndOffset, parent )
    {
    }

    #endregion

  }

}
