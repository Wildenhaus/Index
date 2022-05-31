using System.Collections.Generic;

namespace Saber3D.Data
{

  public class S3DObjectLodRoot
  {

    public List<uint> ObjectIds { get; set; }
    public List<uint> MaxObjectLodIndices { get; set; }
    public List<S3DLodDistance> LodDistances { get; set; }
    public M3DBox BoundingBox { get; set; }

  }

}
