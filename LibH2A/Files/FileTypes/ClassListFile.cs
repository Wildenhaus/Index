namespace Saber3D.Files.FileTypes
{

  [FileSignature( "1SERclass_list" )]
  [FileExtension( ".class_list" )]
  public class ClassListFile : S3DFile
  {

    #region Properties

    public override string FileTypeDisplay => "Class List (.class_list)";

    #endregion

    #region Constructor

    public ClassListFile( string name, H2AStream baseStream,
      long dataStartOffset, long dataEndOffset,
      IS3DFile parent = null )
      : base( name, baseStream, dataStartOffset, dataEndOffset, parent )
    {
    }

    #endregion

  }

}
