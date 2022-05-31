using System.Collections.Generic;

namespace Saber3D.Data
{

  public class S3DAnimSeq
  {

    public string Name { get; set; }
    public uint LayerId { get; set; }
    public float StartFrame { get; set; }
    public float EndFrame { get; set; }
    public float OffsetFrame { get; set; }
    public float LenFrame { get; set; }
    public float TimeSec { get; set; }
    public List<S3DActionFrame> ActionFrames { get; set; }
    public M3DBox BoundingBox { get; set; }

  }

}
