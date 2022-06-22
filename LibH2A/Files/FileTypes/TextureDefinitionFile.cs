namespace Saber3D.Files.FileTypes
{

  [FileExtension( ".td" )]
  public class TextureDefinitionFile : S3DFile
  {

    #region Properties

    public override string FileTypeDisplay => "Texture Definition (.td)";

    #endregion

    #region Constructor

    public TextureDefinitionFile( string name, H2AStream baseStream,
      long dataStartOffset, long dataEndOffset,
      IS3DFile parent = null )
      : base( name, baseStream, dataStartOffset, dataEndOffset, parent )
    {
    }

    #endregion

  }

}
