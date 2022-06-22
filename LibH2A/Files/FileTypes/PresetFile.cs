namespace Saber3D.Files.FileTypes
{

  [FileExtension( ".s3dprs" )]
  [FileExtension( ".grsp" )]
  public class PresetFile : S3DFile
  {


    #region Properties

    public override string FileTypeDisplay => "Preset";

    #endregion

    #region Constructor

    public PresetFile( string name, H2AStream baseStream,
      long dataStartOffset, long dataEndOffset,
      IS3DFile parent = null )
      : base( name, baseStream, dataStartOffset, dataEndOffset, parent )
    {
    }

    #endregion

  }

}
