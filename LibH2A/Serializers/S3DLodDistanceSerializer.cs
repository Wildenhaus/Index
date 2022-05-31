using System.Collections.Generic;
using System.IO;
using Saber3D.Data;

namespace Saber3D.Serializers
{

  public class S3DLodDistanceSerializer : SerializerBase<List<S3DLodDistance>>
  {

    protected override void OnDeserialize( BinaryReader reader, List<S3DLodDistance> lodDists )
    {
      var count = reader.ReadUInt32();
      var propertyCount = reader.ReadUInt32();

      for ( var i = 0; i < count; i++ )
        lodDists.Add( new S3DLodDistance() );

      ReadMaxDistanceProperty( reader, lodDists );
    }

    private void ReadMaxDistanceProperty( BinaryReader reader, List<S3DLodDistance> lodDists )
    {
      if ( reader.ReadByte() == 0 )
        return;

      foreach ( var lodDist in lodDists )
        lodDist.MaxDistance = reader.ReadSingle();
    }

  }

}
