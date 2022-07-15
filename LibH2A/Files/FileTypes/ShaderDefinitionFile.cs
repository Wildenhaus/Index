namespace Saber3D.Files.FileTypes
{

  [FileExtension( ".sd" )]
  public class ShaderDefinitionFile : S3DFile
  {


    #region Properties

    public override string FileTypeDisplay => "Shader Definition (.sd)";

    #endregion

    #region Constructor

    public ShaderDefinitionFile( string name, H2AStream baseStream,
      long dataStartOffset, long dataEndOffset,
      IS3DFile parent = null )
      : base( name, baseStream, dataStartOffset, dataEndOffset, parent )
    {
    }

    #endregion

  }

}
