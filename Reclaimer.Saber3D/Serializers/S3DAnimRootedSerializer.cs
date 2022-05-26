using System.IO;
using Saber3D.Data;

namespace Saber3D.Serializers
{

  public class S3DAnimRootedSerializer : SerializerBase<S3DAnimRooted>
  {

    protected override void OnDeserialize( BinaryReader reader, S3DAnimRooted anim )
    {
      var count = reader.ReadUInt32();
      var unk_01 = reader.ReadUInt16();
      var unk_02 = reader.ReadByte();
    }

  }

}
