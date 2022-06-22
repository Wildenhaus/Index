using Saber3D.Data.Scripting;

namespace Saber3D.Data.Materials
{

  public class S3DMaterial
  {

    [ScriptingProperty( "version" )]
    public int Version { get; set; }

    [ScriptingProperty( "shadingMtl_Tex" )]
    public string ShadingMaterialTexture { get; set; }

    [ScriptingProperty( "shadingMtl_Mtl" )]
    public string ShadingMaterialMaterial { get; set; }

    [ScriptingProperty( "lm" )]
    public S3DMaterialLM LM { get; set; }

    [ScriptingProperty( "layer0" )]
    public S3DMaterialLayer Layer0 { get; set; }

    [ScriptingProperty( "layer1" )]
    public S3DMaterialLayer Layer1 { get; set; }

    [ScriptingProperty( "layer2" )]
    public S3DMaterialLayer Layer2 { get; set; }

    [ScriptingProperty( "layer3" )]
    public S3DMaterialLayer Layer3 { get; set; }

    [ScriptingProperty( "extraParams" )]
    public S3DMaterialExtraParams ExtraParams { get; set; }

    public string MaterialName
    {
      get => $"{ShadingMaterialMaterial}_{ShadingMaterialTexture}";
    }

  }

}
