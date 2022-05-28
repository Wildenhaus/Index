using System;
using System.IO;
using Saber3D.Common;
using Saber3D.Data;

namespace Saber3D.Serializers
{

  public class S3DGeometryGraphSerializer : SerializerBase<S3DGeometryGraph>
  {

    #region Constants

    private const uint SIGNATURE_OGM1 = 0x314D474F; //OGM1

    #endregion

    protected override void OnDeserialize( BinaryReader reader, S3DGeometryGraph graph )
    {
      ReadSignature( reader, SIGNATURE_OGM1 );

      var unk_01 = reader.ReadUInt16();
      var unk_02 = reader.ReadUInt16();
      var unk_03 = reader.ReadUInt16();

      ReadObjectsProperty( reader, graph );
      ReadObjectPropsProperty( reader, graph );
      ReadObjectPsProperty( reader, graph );
      ReadLodRootsProperty( reader, graph );
    }

    private void ReadObjectsProperty( BinaryReader reader, S3DGeometryGraph graph )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      var objectSerializer = new S3DObjectSerializer();
      graph.Objects = objectSerializer.Deserialize( reader );
    }

    private void ReadObjectPropsProperty( BinaryReader reader, S3DGeometryGraph graph )
    {
      // TODO: This is a hack
      // The count and sentinel seem to be shared. That is, if this section exists,
      // the sentinel will be the least significant bit of the count.
      // This section only seems to be present in ss_prop__h.tpl
      // I'd imagine the flags that denote
      var count = reader.ReadByte();
      if ( count != 8 )
        return;

      var unk_01 = reader.ReadByte();
      var unk_02 = reader.ReadByte();
      var unk_03 = reader.ReadByte();
      var unk_04 = reader.ReadByte();

      var props = graph.ObjectProps = new string[ count ];

      for ( var i = 0; i < count; i++ )
      {
        var unk_05 = reader.ReadUInt32();
        props[ i ] = reader.ReadPascalString32();
      }
    }

    private void ReadObjectPsProperty( BinaryReader reader, S3DGeometryGraph graph )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      throw new NotImplementedException();
    }

    private void ReadLodRootsProperty( BinaryReader reader, S3DGeometryGraph graph )
    {
      var serializer = new S3DObjectLodRootSerializer( graph );
      graph.LodRoots = serializer.Deserialize( reader );
    }

  }

}
