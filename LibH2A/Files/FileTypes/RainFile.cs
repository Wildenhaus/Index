namespace Saber3D.Files.FileTypes
{

  [FileSignature( "1SERrain" )]
  [FileExtension( ".rain" )]
  public class RainFile : S3DFile
  {

    #region Properties

    public override string FileTypeDisplay => "Rain (.rain)";

    #endregion

    #region Constructor

    public RainFile( string name, H2AStream baseStream,
      long dataStartOffset, long dataEndOffset,
      IS3DFile parent = null )
      : base( name, baseStream, dataStartOffset, dataEndOffset, parent )
    {
    }

    #endregion

  }

}
