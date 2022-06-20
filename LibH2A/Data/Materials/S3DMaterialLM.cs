using Saber3D.Data.Scripting;

namespace Saber3D.Data.Materials
{

  public class S3DMaterialLM
  {

    [ScriptingProperty( "source" )]
    public string Source { get; set; }

    [ScriptingProperty( "texName" )]
    public string TextureName { get; set; }

    [ScriptingProperty( "uvSetIdx" )]
    public int UvSetIndex { get; set; }

    [ScriptingProperty( "tangent" )]
    public S3DMaterialTangent Tangent { get; set; }

  }

}
