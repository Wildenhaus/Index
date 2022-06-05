using System.Collections.Generic;
using System.IO;
using Saber3D.Data;
using static Saber3D.Assertions;

namespace Saber3D.Serializers
{

  public class S3DGeometryMeshSerializer : SerializerBase<List<S3DGeometryMesh>>
  {

    private S3DGeometryGraph GeometryGraph { get; }

    public S3DGeometryMeshSerializer( S3DGeometryGraph geometryGraph )
    {
      GeometryGraph = geometryGraph;
    }

    protected override void OnDeserialize( BinaryReader reader, List<S3DGeometryMesh> meshes )
    {
      var count = GeometryGraph.MeshCount;
      var sectionEndOffset = reader.ReadUInt32();

      for ( var i = 0; i < count; i++ )
        meshes.Add( new S3DGeometryMesh() { Id = ( uint ) i } );

      while ( reader.BaseStream.Position < sectionEndOffset )
      {
        var sentinel = ( MeshSentinel ) reader.ReadUInt16();
        var endOffset = reader.ReadUInt32();

        switch ( sentinel )
        {
          case MeshSentinel.MeshFlags:
            ReadMeshFlags( reader, meshes );
            break;
          case MeshSentinel.BufferInfo:
            ReadBufferInfo( reader, meshes );
            break;
          default:
            Fail( $"Unknown Mesh Sentinel: {sentinel:X}" );
            break;
        }

        Assert( reader.BaseStream.Position == endOffset,
          "Reader position does not match the mesh sentinel's end offset." );
      }

      Assert( reader.BaseStream.Position == sectionEndOffset,
          "Reader position does not match the mesh section's end offset." );
    }

    private void ReadMeshFlags( BinaryReader reader, List<S3DGeometryMesh> meshes )
    {
      foreach ( var mesh in meshes )
      {
        mesh.FlagCount = reader.ReadUInt16();
        mesh.Flags = ( S3DGeometryMeshFlags ) reader.ReadUInt64();
      }
    }

    private void ReadBufferInfo( BinaryReader reader, List<S3DGeometryMesh> meshes )
    {
      foreach ( var mesh in meshes )
      {
        var count = reader.ReadByte();
        var buffers = new S3DGeometryMeshBuffer[ count ];

        for ( var i = 0; i < count; i++ )
        {
          buffers[ i ] = new S3DGeometryMeshBuffer
          {
            BufferId = reader.ReadUInt32(),
            SubBufferOffset = reader.ReadUInt32()
          };
        }

        mesh.Buffers = buffers;
      }
    }

    private enum MeshSentinel : ushort
    {
      MeshFlags = 0x0000,
      // TODO: 0x0001 doesn't seem to be used anywhere. Verify.
      BufferInfo = 0x0002
    }

  }

}
