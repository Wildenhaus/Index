namespace Saber3D.Files.FileTypes
{

  [FileExtension( ".dsh" )]
  [FileExtension( ".fx" )]
  [FileExtension( ".hsh" )]
  [FileExtension( ".psh" )]
  [FileExtension( ".vsh" )]
  public class ShaderCodeFile : S3DFile
  {


    #region Properties

    public override string FileTypeDisplay => "Shader Code";

    #endregion

    #region Constructor

    public ShaderCodeFile( string name, H2AStream baseStream,
      long dataStartOffset, long dataEndOffset,
      IS3DFile parent = null )
      : base( name, baseStream, dataStartOffset, dataEndOffset, parent )
    {
    }

    #endregion

  }

}
