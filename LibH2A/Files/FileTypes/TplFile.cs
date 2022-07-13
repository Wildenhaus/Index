namespace Saber3D.Files.FileTypes
{

  [FileSignature( "1SERtpl" )]
  [FileExtension( ".tpl" )]
  public class TplFile : S3DFile
  {

    #region Properties

    public override string FileTypeDisplay => "Model (.tpl)";

    #endregion

    #region Constructor

    public TplFile( string name, H2AStream baseStream,
      long dataStartOffset, long dataEndOffset,
      IS3DFile parent = null )
      : base( name, baseStream, dataStartOffset, dataEndOffset, parent )
    {
    }

    #endregion

  }

}