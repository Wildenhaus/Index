namespace Saber3D.Files.FileTypes
{

  [FileExtension( ".ssc" )]
  [FileExtension( ".ssl" )]
  [FileExtension( ".ps" )]
  [FileExtension( ".fsm" )]
  public class ScriptingFile : S3DFile
  {


    #region Properties

    public override string FileTypeDisplay => "Script";

    #endregion

    #region Constructor

    public ScriptingFile( string name, H2AStream baseStream,
      long dataStartOffset, long dataEndOffset,
      IS3DFile parent = null )
      : base( name, baseStream, dataStartOffset, dataEndOffset, parent )
    {
    }

    #endregion

  }

}
