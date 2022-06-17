using Saber3D.Data.Shared;

namespace Saber3D.Data.Materials
{

  public class S3DMaterialAuxiliaryTextures
  {

    [ConfigurationProperty( "mask" )]
    public S3DMaterialMask Mask { get; set; }

  }

}
