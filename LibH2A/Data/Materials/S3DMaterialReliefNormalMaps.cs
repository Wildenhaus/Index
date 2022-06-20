using Saber3D.Data.Scripting;

namespace Saber3D.Data.Materials
{

  public class S3DMaterialReliefNormalMaps
  {

    [ScriptingProperty( "macro" )]
    public S3DMaterialNormalMap Macro { get; set; }

    [ScriptingProperty( "micro1" )]
    public S3DMaterialNormalMap Micro1 { get; set; }

    [ScriptingProperty( "micro2" )]
    public S3DMaterialNormalMap Micro2 { get; set; }

  }

}
