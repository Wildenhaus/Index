using Saber3D.Data.Scripting;

namespace Saber3D.Data.Materials
{

  public class S3DMaterialHeightMap
  {

    [ScriptingProperty( "colorSetIdx" )]
    public int ColorSetIndex { get; set; }

    [ScriptingProperty( "invert" )]
    public bool Invert { get; set; }

  }

}
