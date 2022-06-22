using Saber3D.Data.Textures;
using Saber3D.Serializers;

namespace Saber3D.Files.FileTypes
{

  [FileExtension( ".pct" )]
  public class PictureFile : S3DFile
  {

    #region Properties

    public override string FileTypeDisplay => "Texture (.pct)";

    #endregion

    #region Constructor

    public PictureFile( string name, H2AStream baseStream,
      long dataStartOffset, long dataEndOffset,
      IS3DFile parent = null )
      : base( name, baseStream, dataStartOffset, dataEndOffset, parent )
    {
    }

    #endregion

    #region Public Methods

    public S3DPicture Deserialize()
    {
      var stream = GetStream();
      try
      {
        stream.AcquireLock();
        return S3DPictureSerializer.Deserialize( stream );
      }
      finally { stream.ReleaseLock(); }
    }

    #endregion

  }

}
