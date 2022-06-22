using Saber3D.Data;
using Saber3D.Serializers;

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

    #region Public Methods

    public S3DTemplate Deserialize()
    {
      var stream = GetStream();
      try
      {
        stream.AcquireLock();
        return S3DTemplateSerializer.Deserialize( stream );
      }
      finally { stream.ReleaseLock(); }
    }

    #endregion

  }

}