using System.Numerics;
using Saber3D.Data.Materials;

namespace Saber3D.Data
{

  public class S3DGeometrySubMesh
  {

    public S3DSubMeshBufferInfo BufferInfo { get; set; }
    public uint MeshId { get; set; } // TODO: This is a guess

    public Vector3? Position { get; set; }
    public Vector3? Scale { get; set; }

    public ushort NodeId { get; set; }
    public S3DMaterial Material { get; set; }

    public ushort[] BoneIds { get; set; }

  }

  public struct S3DSubMeshBufferInfo
  {
    public ushort VertexOffset;
    public ushort VertexCount;
    public ushort FaceOffset;
    public ushort FaceCount;
    public ushort NodeId;
    public ushort SkinCompoundId;
  }

}
