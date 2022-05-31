namespace Saber3D.Data.Materials
{

  public class S3DMaterialExtraParams
  {

    [MaterialProperty( "reliefNormalmaps" )]
    public S3DMaterialReliefNormalMaps ReliefNormalMaps { get; set; }

    [MaterialProperty( "auxiliaryTextures" )]
    public S3DMaterialAuxiliaryTextures AuxiliaryTextures { get; set; }

    [MaterialProperty( "transparency" )]
    public S3DMaterialTransparency Transparency { get; set; }

    [MaterialProperty( "extraVertexColorData" )]
    public S3DMaterialExtraVertexColorData ExtraVertexColorData { get; set; }

  }

}
