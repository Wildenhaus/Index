using Saber3D.Data.Scripting;

namespace Saber3D.Data.Materials
{

  public class S3DMaterialExtraParams
  {

    [ScriptingProperty( "reliefNormalmaps" )]
    public S3DMaterialReliefNormalMaps ReliefNormalMaps { get; set; }

    [ScriptingProperty( "auxiliaryTextures" )]
    public S3DMaterialAuxiliaryTextures AuxiliaryTextures { get; set; }

    [ScriptingProperty( "transparency" )]
    public S3DMaterialTransparency Transparency { get; set; }

    [ScriptingProperty( "extraVertexColorData" )]
    public S3DMaterialExtraVertexColorData ExtraVertexColorData { get; set; }

  }

}
