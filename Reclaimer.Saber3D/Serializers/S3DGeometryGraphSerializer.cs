using System;
using System.IO;
using Saber3D.Common;
using Saber3D.Data;
using static Saber3D.Assertions;

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

      // TODO: These are guesses.
      var graphType = ( GraphType ) reader.ReadUInt16();
      var unk_02 = reader.ReadUInt16(); // TODO
      var unk_03 = reader.ReadUInt16(); // TODO

      //if ( unk_01 != 1 )
      //  System.Diagnostics.Debugger.Break();
      //if ( unk_02 != 2 )
      //  System.Diagnostics.Debugger.Break();
      //if ( unk_03 != 3 && unk_03 != 2 )
      //  System.Diagnostics.Debugger.Break();

      ReadObjectsProperty( reader, graph );

      if ( graphType == GraphType.Props )
        ReadObjectPropsProperty( reader, graph );
      //ReadObjectPsProperty( reader, graph );

      if ( graphType == GraphType.Grass )
        ReadLodRootsProperty( reader, graph );

      ReadData( reader, graph );
    }

    #region Property Read Methods

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
      // This section only seems to be present in ss_prop__h.tpl
      var count = reader.ReadByte();

      var unk_01 = reader.ReadByte(); // TODO
      var unk_02 = reader.ReadByte(); // TODO
      var unk_03 = reader.ReadByte(); // TODO
      var unk_04 = reader.ReadByte(); // TODO

      var props = graph.ObjectProps = new string[ count ];

      for ( var i = 0; i < count; i++ )
      {
        var unk_05 = reader.ReadUInt32(); // TODO
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
      var serializer = new S3DObjectLodRootSerializer();
      graph.LodRoots = serializer.Deserialize( reader );
    }

    #endregion

    #region Data Read Methods

    private void ReadData( BinaryReader reader, S3DGeometryGraph graph )
    {
      /* Reads the geometry data.
       * NOTE: Everything inside of here is subjectively named and interpreted.
       * As of writing this method, there is very little material to go on in terms
       * of RTTI or strings.
       * 
       * This data seems to be stored as streams and are accessed as needed.
       * 
       * RTTI of interest:
       *  - rendPROP_GLT
       *  - dsTYPE_STORAGE_ITEM_T<rendPROP_GLT>
       *  
       * Strings of interest:
       *  - OBJ_GEOM_STRM_BONE_INDEX
       *  - OBJ_GEOM_STRM_WEIGHT
       *  - OBJ_GEOM_STRM_FACE
       *  - OBJ_GEOM_STRM_VERT
       *  - OBJ_GEOM_STRM_INSTANCED
       *  - OBJ_GEOM_STRM_INTERLEAVED
       */
      while ( true )
      {
        var sentinel = ( DataSentinel ) reader.ReadInt16();

        switch ( sentinel )
        {
          case DataSentinel.Header:
            ReadHeaderData( reader, graph );
            break;
          case DataSentinel.Buffers:
            ReadBufferData( reader, graph );
            break;
          case DataSentinel.Meshes:
            ReadMeshData( reader, graph );
            break;
          case DataSentinel.SubMeshes:
            ReadSubMeshData( reader, graph );
            break;
          case DataSentinel.EndOfData:
            ReadEndOfData( reader, graph );
            return;
          default:
            Fail( $"Unknown GeometryGraph Data Sentinel: {sentinel:X}" );
            break;
        }
      }
    }

    private void ReadHeaderData( BinaryReader reader, S3DGeometryGraph graph )
    {
      var endOffset = reader.ReadUInt32();

      graph.RootNodeIndex = reader.ReadInt16();
      graph.NodeCount = reader.ReadUInt32();
      graph.BufferCount = reader.ReadUInt32();
      graph.MeshCount = reader.ReadUInt32();
      graph.SubMeshCount = reader.ReadUInt32();

      var unk_01 = reader.ReadUInt32(); // TODO
      var unk_02 = reader.ReadUInt32(); // TODO

      Assert( reader.BaseStream.Position == endOffset,
          "Reader position does not match data header's end offset." );
    }

    private void ReadBufferData( BinaryReader reader, S3DGeometryGraph graph )
    {
      var serializer = new S3DGeometryBufferSerializer( graph );
      graph.Buffers = serializer.Deserialize( reader );
    }

    private void ReadMeshData( BinaryReader reader, S3DGeometryGraph graph )
    {
      var serializer = new S3DGeometryMeshSerializer( graph );
      graph.Meshes = serializer.Deserialize( reader );
    }

    private void ReadSubMeshData( BinaryReader reader, S3DGeometryGraph graph )
    {
      var serializer = new S3DGeometrySubMeshSerializer( graph );
      graph.SubMeshes = serializer.Deserialize( reader );
    }

    private void ReadEndOfData( BinaryReader reader, S3DGeometryGraph graph )
    {
      var endOffset = reader.ReadUInt32();
      Assert( reader.BaseStream.Position == endOffset,
          "Reader position does not match data's end offset." );
    }

    #endregion

    #region Embedded Types

    private enum GraphType : ushort
    {
      Default = 1,
      Props = 3,
      Grass = 4
    }

    private enum DataSentinel : ushort
    {
      Header = 0x0000,
      Buffers = 0x0002,
      Meshes = 0x0003,
      SubMeshes = 0x0004,
      EndOfData = 0xFFFF
    }

    #endregion

  }

}
