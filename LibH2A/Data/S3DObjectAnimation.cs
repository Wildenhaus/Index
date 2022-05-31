using System.Numerics;

namespace Saber3D.Data
{

  public class S3DObjectAnimation
  {
    public Vector3 IniTranslation { get; set; }
    public M3DSpline PTranslation { get; set; }
    public Vector4 IniRotation { get; set; }
    public M3DSpline PRotation { get; set; }
    public Vector3 IniScale { get; set; }
    public M3DSpline PScale { get; set; }
    public float IniVisibility { get; set; }
    public M3DSpline PVisibility { get; set; }
  }

}
