using Saber3D.Data.Shared;

namespace Saber3D.Data.Materials
{

  public class S3DMaterial
  {

    [ConfigurationProperty( "version" )]
    public int Version { get; set; }

    [ConfigurationProperty( "shadingMtl_Tex" )]
    public string ShadingMaterialTexture { get; set; }

    [ConfigurationProperty( "shadingMtl_Mtl" )]
    public string ShadingMaterialMaterial { get; set; }

    [ConfigurationProperty( "lm" )]
    public S3DMaterialLM LM { get; set; }

    [ConfigurationProperty( "layer0" )]
    public S3DMaterialLayer Layer0 { get; set; }

    [ConfigurationProperty( "layer1" )]
    public S3DMaterialLayer Layer1 { get; set; }

    [ConfigurationProperty( "layer2" )]
    public S3DMaterialLayer Layer2 { get; set; }

    [ConfigurationProperty( "layer3" )]
    public S3DMaterialLayer Layer3 { get; set; }

    [ConfigurationProperty( "extraParams" )]
    public S3DMaterialExtraParams ExtraParams { get; set; }

  }

}
