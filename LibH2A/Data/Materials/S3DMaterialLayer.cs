using Saber3D.Data.Scripting;

namespace Saber3D.Data.Materials
{

  public class S3DMaterialLayer
  {

    [ScriptingProperty( "texName" )]
    public string TextureName { get; set; }

    [ScriptingProperty( "mtlName" )]
    public string MaterialName { get; set; }

    [ScriptingProperty( "tint" )]
    public float[] Tint { get; set; }

    [ScriptingProperty( "vcSet" )]
    public int VcSet { get; set; }

    [ScriptingProperty( "tilingU" )]
    public float TilingU { get; set; }

    [ScriptingProperty( "tilingV" )]
    public float TilingV { get; set; }

    [ScriptingProperty( "blending" )]
    public S3DMaterialBlending Blending { get; set; }

    [ScriptingProperty( "uvSetIdx" )]
    public int UvSetIndex { get; set; }

  }

}
