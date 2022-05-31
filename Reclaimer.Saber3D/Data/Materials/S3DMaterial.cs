namespace Saber3D.Data.Materials
{

  public class S3DMaterial
  {

    [MaterialProperty( "version" )]
    public int Version { get; set; }

    [MaterialProperty( "shadingMtl_Tex" )]
    public string ShadingMaterialTexture { get; set; }

    [MaterialProperty( "shadingMtl_Mtl" )]
    public string ShadingMaterialMaterial { get; set; }

    [MaterialProperty( "lm" )]
    public S3DMaterialLM LM { get; set; }

    [MaterialProperty( "layer0" )]
    public S3DMaterialLayer Layer0 { get; set; }

    [MaterialProperty( "layer1" )]
    public S3DMaterialLayer Layer1 { get; set; }

    [MaterialProperty( "layer2" )]
    public S3DMaterialLayer Layer2 { get; set; }

    [MaterialProperty( "layer3" )]
    public S3DMaterialLayer Layer3 { get; set; }

    [MaterialProperty( "extraParams" )]
    public S3DMaterialExtraParams ExtraParams { get; set; }

  }

}
