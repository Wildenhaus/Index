namespace Saber3D.Files.FileTypes
{

  public class GenericTextFile : S3DFile
  {

    #region Properties

    public override string FileTypeDisplay => "Text File";

    #endregion

    #region Constructor

    public GenericTextFile( string name, H2AStream baseStream,
      long dataStartOffset, long dataEndOffset,
      IS3DFile parent = null )
      : base( name, baseStream, dataStartOffset, dataEndOffset, parent )
    {
    }

    #endregion

  }

}
