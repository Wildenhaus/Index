using System.IO;
using Saber3D.Common;
using Saber3D.Data;

namespace Saber3D.Serializers
{

  public class S3DAnimRootedSerializer : SerializerBase<S3DAnimRooted>
  {

    protected override void OnDeserialize( BinaryReader reader, S3DAnimRooted anim )
    {
      var unk_01 = reader.ReadUInt32();
      var propertyFlags = reader.ReadBitArray( 4 );

      if ( propertyFlags[ 0 ] )
        reader.ReadVector3();
      if ( propertyFlags[ 1 ] )
      {
        var serializer = new M3DSplineSerializer();
        serializer.Deserialize( reader );
      }
      if ( propertyFlags[ 2 ] )
        reader.ReadVector4();
      if ( propertyFlags[ 4 ] )
      {
        var serializer = new M3DSplineSerializer();
        serializer.Deserialize( reader );
      }

    }

  }

}
