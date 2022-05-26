using System.IO;
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
      //ReadObjectPropsProperty( reader, graph );
      //ReadObjectPsProperty( reader, graph );
      //ReadLodRootsProperty( reader, graph );
    }

    private void ReadObjectsProperty( BinaryReader reader, S3DGeometryGraph graph )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      var objectSerializer = new S3DObjectSerializer();
      graph.Objects = objectSerializer.Deserialize( reader );
    }

    //private void ReadObjectPropsProperty( BinaryReader reader, S3DGeometryGraph graph )
    //{
    //  // Read Sentinel
    //  if ( reader.ReadByte() == 0 )
    //    return;

    //  throw new NotImplementedException();
    //}

    //private void ReadObjectPsProperty( BinaryReader reader, S3DGeometryGraph graph )
    //{
    //  // Read Sentinel
    //  if ( reader.ReadByte() == 0 )
    //    return;

    //  throw new NotImplementedException();
    //}

    //private void ReadLodRootsProperty( BinaryReader reader, S3DGeometryGraph graph )
    //{
    //}

  }

}
