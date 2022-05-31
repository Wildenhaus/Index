using System.Numerics;

namespace Saber3D.Data
{

  public class S3DObject
  {

    public short Id { get; set; }
    public string ReadName { get; set; }

    public short ParentId { get; set; }
    public short NextId { get; set; }
    public short PrevId { get; set; }
    public short ChildId { get; set; }

    public short AnimNumber { get; set; }
    public string ReadAffixes { get; set; }

    public Matrix4x4 MatrixLT { get; set; }
    public Matrix4x4 MatrixModel { get; set; }

    public S3DObjectGeometryUnshared GeomData { get; set; }

    public string UnkName { get; set; }
    // OBB
    public string Name { get; set; }
    public string Affixes { get; set; }
  }

}
