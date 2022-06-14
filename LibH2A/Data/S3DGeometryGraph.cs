using System.Collections.Generic;

namespace Saber3D.Data
{

  public class S3DGeometryGraph
  {

    public List<S3DObject> Objects { get; set; }
    public string[] ObjectProps { get; set; }
    public List<S3DObjectLodRoot> LodRoots { get; set; }

    public short RootNodeIndex { get; set; }
    public uint NodeCount { get; set; }
    public uint BufferCount { get; set; }
    public uint MeshCount { get; set; }
    public uint SubMeshCount { get; set; }

    public List<S3DGeometryBuffer> Buffers { get; set; }
    public List<S3DGeometryMesh> Meshes { get; set; }
    public List<S3DGeometrySubMesh> SubMeshes { get; set; }

    public S3DObject RootObject
    {
      get => Objects[ RootNodeIndex ];
    }

  }

}
