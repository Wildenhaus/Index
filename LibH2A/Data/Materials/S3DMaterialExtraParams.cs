using Saber3D.Data.Shared;

namespace Saber3D.Data.Materials
{

  public class S3DMaterialExtraParams
  {

    [ConfigurationProperty( "reliefNormalmaps" )]
    public S3DMaterialReliefNormalMaps ReliefNormalMaps { get; set; }

    [ConfigurationProperty( "auxiliaryTextures" )]
    public S3DMaterialAuxiliaryTextures AuxiliaryTextures { get; set; }

    [ConfigurationProperty( "transparency" )]
    public S3DMaterialTransparency Transparency { get; set; }

    [ConfigurationProperty( "extraVertexColorData" )]
    public S3DMaterialExtraVertexColorData ExtraVertexColorData { get; set; }

  }

}
