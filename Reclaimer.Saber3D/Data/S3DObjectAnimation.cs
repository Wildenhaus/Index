using System.Numerics;

namespace Saber3D.Data
{

  public class S3DObjectAnimation
  {
    public Vector3 IniTranslation { get; set; }
    public object PTranslation { get; set; }
    public Vector4 IniRotation { get; set; }
    public object PRotation { get; set; }
    public Vector3 IniScale { get; set; }
    public object PScale { get; set; }
    public float IniVisibility { get; set; }
    public object PVisibility { get; set; }
  }

}
