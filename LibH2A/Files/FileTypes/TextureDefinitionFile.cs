using System.IO;

namespace Saber3D.Files.FileTypes
{

  [FileExtension( ".td" )]
  public class TextureDefinitionFile : S3DFile
  {

    #region Properties

    public override string FileTypeDisplay => "Texture Definition (.td)";

    #endregion

    #region Constructor

    public TextureDefinitionFile( string name, Stream stream, IS3DFile parent = null )
      : base( name, stream, parent )
    {
    }

    #endregion

  }

}
