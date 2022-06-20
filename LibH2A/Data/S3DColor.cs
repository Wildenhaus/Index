using Saber3D.Data.Scripting;

namespace Saber3D.Data
{

  public class S3DColor
  {

    [ScriptingProperty( "R", arrayIndex: 0 )]
    public float R { get; set; }

    [ScriptingProperty( "G", arrayIndex: 1 )]
    public float G { get; set; }

    [ScriptingProperty( "B", arrayIndex: 2 )]
    public float B { get; set; }

    [ScriptingProperty( "A", arrayIndex: 3 )]
    public float A { get; set; }

  }

}
