using System;
using System.IO;
using Saber3D.Common;
using Saber3D.Data;

namespace Saber3D.Serializers
{

  public class S3DAnimRootedSerializer : SerializerBase<S3DAnimRooted>
  {

    protected override void OnDeserialize( BinaryReader reader, S3DAnimRooted anim )
    {
      var propertyCount = reader.ReadUInt32();
      var properties = ( AnimRootedProperty ) reader.ReadByte();

      if ( properties.HasFlag( AnimRootedProperty.IniTranslation ) )
        anim.IniTranslation = reader.ReadVector3();

      if ( properties.HasFlag( AnimRootedProperty.PTranslation ) )
      {
        var serializer = new M3DSplineSerializer();
        anim.PTranslation = serializer.Deserialize( reader );
      }

      if ( properties.HasFlag( AnimRootedProperty.IniRotation ) )
        anim.IniRotation = reader.ReadVector4();

      if ( properties.HasFlag( AnimRootedProperty.PRotation ) )
      {
        var serializer = new M3DSplineSerializer();
        anim.PRotation = serializer.Deserialize( reader );
      }

    }

    [Flags]
    private enum AnimRootedProperty : byte
    {
      IniTranslation = 1 << 0,
      PTranslation = 1 << 1,
      IniRotation = 1 << 2,
      PRotation = 1 << 3
    }

  }

}
