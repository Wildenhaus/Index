using System.Collections.Generic;

namespace Saber3D.Data
{

  public class S3DGeometryGraph
  {

    public List<S3DObject> Objects { get; set; }
    public string[] ObjectProps { get; set; }
    public List<S3DObjectLodRoot> LodRoots { get; set; }

  }

}
