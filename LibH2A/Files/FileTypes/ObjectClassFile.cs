namespace Saber3D.Files.FileTypes
{

  [FileExtension( ".cls" )]
  public class ObjectClassFile : S3DFile
  {


    #region Properties

    public override string FileTypeDisplay => "Object Class";

    #endregion

    #region Constructor

    public ObjectClassFile( string name, H2AStream baseStream,
      long dataStartOffset, long dataEndOffset,
      IS3DFile parent = null )
      : base( name, baseStream, dataStartOffset, dataEndOffset, parent )
    {
    }

    #endregion

  }

}
