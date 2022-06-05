using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Saber3D.Common;
using Saber3D.Data;
using Saber3D.Data.Materials;
using Saber3D.Serializers.Materials;
using static Saber3D.Assertions;

namespace Saber3D.Serializers
{

  public class S3DGeometrySubMeshSerializer : SerializerBase<List<S3DGeometrySubMesh>>
  {

    private S3DGeometryGraph GeometryGraph { get; }

    public S3DGeometrySubMeshSerializer( S3DGeometryGraph geometryGraph )
    {
      GeometryGraph = geometryGraph;
    }

    protected override void OnDeserialize( BinaryReader reader, List<S3DGeometrySubMesh> submeshes )
    {
      var count = GeometryGraph.SubMeshCount;
      var sectionEndOffset = reader.ReadUInt32();

      for ( var i = 0; i < count; i++ )
        submeshes.Add( new S3DGeometrySubMesh() );

      while ( reader.BaseStream.Position < sectionEndOffset )
      {
        var sentinel = ( SubMeshSentinel ) reader.ReadUInt16();
        var endOffset = reader.ReadUInt32();

        switch ( sentinel )
        {
          case SubMeshSentinel.BufferInfo:
            ReadBufferInfo( reader, submeshes, endOffset );
            break;
          case SubMeshSentinel.MeshIds:
            ReadMeshIds( reader, submeshes );
            break;
          case SubMeshSentinel.Unk_02:
            ReadUnk02( reader, submeshes );
            break;
          case SubMeshSentinel.BoneMap:
            ReadBoneMap( reader, submeshes, endOffset );
            break;
          case SubMeshSentinel.SubMeshInfo:
            ReadSubMeshInfo( reader, submeshes, endOffset );
            break;
          case SubMeshSentinel.TransformInfo:
            ReadTransformInfo( reader, submeshes );
            break;
          case SubMeshSentinel.Materials_String:
            ReadMaterialsString( reader, submeshes );
            break;
          case SubMeshSentinel.Materials_Static:
            ReadMaterialsStatic( reader, submeshes );
            break;
          case SubMeshSentinel.Materials_Dynamic:
            ReadMaterialsDynamic( reader, submeshes );
            break;
          default:
            Fail( $"Unknown SubMesh Sentinel: {sentinel:X}" );
            break;
        }

        Assert( reader.BaseStream.Position == endOffset,
          "Reader position does not match the submesh sentinel's end offset." );
      }

      Assert( reader.BaseStream.Position == sectionEndOffset,
          "Reader position does not match the submesh section's end offset." );
    }

    private void ReadBufferInfo( BinaryReader reader, List<S3DGeometrySubMesh> submeshes, long endOffset )
    {
      // TODO: This is a bit hacky in order to determine whether or not we should do the seek nonsense.
      if ( submeshes.Count == 0 )
        return;

      const int BUFFER_INFO_SIZE = 0xC;
      var sectionSize = endOffset - reader.BaseStream.Position;
      var elementSize = sectionSize / submeshes.Count;
      bool hasAdditionalData = elementSize > BUFFER_INFO_SIZE;

      foreach ( var submesh in submeshes )
      {
        submesh.BufferInfo = new S3DSubMeshBufferInfo
        {
          VertexOffset = reader.ReadUInt16(),
          VertexCount = reader.ReadUInt16(),
          FaceOffset = reader.ReadUInt16(),
          FaceCount = reader.ReadUInt16(),
          NodeId = reader.ReadUInt16(),
          SkinCompoundId = reader.ReadUInt16()
        };

        /* Not sure what this data is, but we're just gonna seek past it for now.
         * It has something to do with a flag set earlier in the file.
         * This implementation of looping 32-bits and doing a left rotate seems to be correct.
         * That's how it's done in the disassembly.
         */
        if ( hasAdditionalData )
        {
          var seekFlags = reader.ReadUInt32();

          for ( var i = 0; i < 32; i++ )
          {
            if ( ( seekFlags & 1 ) != 0 )
            {
              // These appear to be floats
              var a = reader.ReadSingle();
              var b = reader.ReadSingle();
            }
            seekFlags = ( seekFlags << 1 ) | ( seekFlags >> 31 );
          }
        }
      }
    }

    private void ReadMeshIds( BinaryReader reader, List<S3DGeometrySubMesh> submeshes )
    {
      foreach ( var submesh in submeshes )
        submesh.MeshId = reader.ReadUInt32();
    }

    private void ReadUnk02( BinaryReader reader, List<S3DGeometrySubMesh> submeshes )
    {
      // TODO
      /* Not sure what this is. Just like the BufferInfo section, 
       * this does the same weird reading pattern at the end.
       * Seems to be a nested section and some scripting.
       */
      // TODO: Move this to own serializer once we figure out that it is

      foreach ( var submesh in submeshes )
      {
        var unk_01 = reader.ReadUInt16();
        var count = reader.ReadByte();

        while ( true )
        {
          var sentinel = reader.ReadUInt16();
          var endOffset = reader.ReadUInt32();

          switch ( sentinel )
          {
            case 0x0000:
            {
              for ( var i = 0; i < count; i++ )
              {
                _ = reader.ReadInt16();
                _ = reader.ReadInt16();
                _ = reader.ReadByte();
                _ = reader.ReadByte();
              }
            }
            break;
            case 0x0001:
            {
              for ( var i = 0; i < count; i++ )
              {
                _ = reader.ReadByte();
                _ = reader.ReadByte();
              }
            }
            break;
            case 0x0002:
            {
              for ( var i = 0; i < count; i++ )
              {
                _ = reader.ReadByte();
                _ = reader.ReadByte();
              }
            }
            break;
            case 0x0003:
            {
              for ( var i = 0; i < count; i++ )
                _ = reader.ReadPascalString32();
            }
            break;
            case 0xFFFF:
              break;
            default:
              Fail( $"Unknown sentinel: {sentinel:X2}" );
              break;
          }

          Assert( reader.BaseStream.Position == endOffset );

          if ( sentinel == 0xFFFF )
            break;
        }

        var seekFlags = reader.ReadUInt32();
        for ( var i = 0; i < 32; i++ )
        {
          if ( ( seekFlags & 1 ) != 0 )
          {
            // These appear to be floats
            var a = reader.ReadSingle();
            var b = reader.ReadSingle();
          }
          seekFlags = ( seekFlags << 1 ) | ( seekFlags >> 31 );
        }

      }

    }

    private void ReadBoneMap( BinaryReader reader, List<S3DGeometrySubMesh> submeshes, long endOffset )
    {
      // TODO: Figure out if this is actually a bone map.
      // TODO: How does this factor in to each submesh?

      while ( reader.BaseStream.Position < endOffset )
      {
        var count = reader.ReadByte();
        for ( var i = 0; i < count; i++ )
          reader.ReadUInt16();
      }
    }

    private void ReadSubMeshInfo( BinaryReader reader, List<S3DGeometrySubMesh> submeshes, long endOffset )
    {
      // TODO: Figure out what this data is.

      while ( reader.BaseStream.Position < endOffset )
      {
        var count = reader.ReadByte();
        for ( var i = 0; i < count; i++ )
        {
          var unk_01 = reader.ReadByte(); // TODO
          var unk_02 = reader.ReadUInt16(); // TODO
        }
      }
    }

    private void ReadTransformInfo( BinaryReader reader, List<S3DGeometrySubMesh> submeshes )
    {
      // TODO:
      /* This is probably transform data, but sometimes it behaves inconsistently when
       * applied to a mesh vs level geometry.
       */

      foreach ( var submesh in submeshes )
      {
        var mesh = GeometryGraph.Meshes[ ( int ) submesh.MeshId ];
        if ( !mesh.Flags.HasFlag( S3DGeometryMeshFlags.HasTransformInfo ) )
          continue;

        submesh.Position = new Vector3(
          x: reader.ReadInt16().SNormToFloat(),
          y: reader.ReadInt16().SNormToFloat(),
          z: reader.ReadInt16().SNormToFloat()
        );

        submesh.Scale = new Vector3(
          x: reader.ReadInt16(),
          y: reader.ReadInt16(),
          z: reader.ReadInt16()
        );
      }
    }

    private void ReadMaterialsString( BinaryReader reader, List<S3DGeometrySubMesh> submeshes )
    {
      /* Materials with this sentinel are represented as just a string.
       */
      var serializer = new StringMaterialSerializer<S3DMaterial>();
      foreach ( var submesh in submeshes )
      {
        submesh.NodeId = reader.ReadUInt16();
        submesh.Material = serializer.Deserialize( reader );
      }
    }

    private void ReadMaterialsStatic( BinaryReader reader, List<S3DGeometrySubMesh> submeshes )
    {
      /* Materials with this sentinel are similar to the MaterialsDynamic section, but the strings 
       * are null-terminated and there's an additional int32 after the property name.
       */
      var serializer = new StaticMaterialSerializer<S3DMaterial>();
      foreach ( var submesh in submeshes )
      {
        submesh.NodeId = reader.ReadUInt16();
        submesh.Material = serializer.Deserialize( reader );
      }
    }

    private void ReadMaterialsDynamic( BinaryReader reader, List<S3DGeometrySubMesh> submeshes )
    {
      /* This is the most common form of materials.
       * The properties are pascal strings, followed by the data type, and then the value.
       */
      var serializer = new DynamicMaterialSerializer<S3DMaterial>();
      foreach ( var submesh in submeshes )
      {
        submesh.NodeId = reader.ReadUInt16();
        submesh.Material = serializer.Deserialize( reader );
      }
    }

    private enum SubMeshSentinel : ushort
    {
      BufferInfo = 0x0000,
      MeshIds = 0x0001,
      Unk_02 = 0x0002,
      BoneMap = 0x0003,
      SubMeshInfo = 0x0004,
      TransformInfo = 0x0005,
      Materials_String = 0x0006,
      Materials_Static = 0x0007,
      Materials_Dynamic = 0x0008
    }

  }

}
