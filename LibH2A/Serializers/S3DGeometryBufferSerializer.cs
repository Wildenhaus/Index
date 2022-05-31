using System.Collections.Generic;
using System.IO;
using Saber3D.Data;
using static Saber3D.Assertions;

namespace Saber3D.Serializers
{

  public class S3DGeometryBufferSerializer : SerializerBase<List<S3DGeometryBuffer>>
  {

    private S3DGeometryGraph GeometryGraph { get; }

    public S3DGeometryBufferSerializer( S3DGeometryGraph geometryGraph )
    {
      GeometryGraph = geometryGraph;
    }

    protected override void OnDeserialize( BinaryReader reader, List<S3DGeometryBuffer> buffers )
    {
      var count = GeometryGraph.BufferCount;
      var sectionEndOffset = reader.ReadUInt32();

      for ( var i = 0; i < count; i++ )
        buffers.Add( new S3DGeometryBuffer() );

      while ( reader.BaseStream.Position < sectionEndOffset )
      {
        var sentinel = ( BufferSentinel ) reader.ReadUInt16();
        var endOffset = reader.ReadUInt32();

        switch ( sentinel )
        {
          case BufferSentinel.BufferTypeInfo:
            ReadBufferTypeInfo( reader, buffers );
            break;
          case BufferSentinel.BufferElementSizes:
            ReadBufferElementSizes( reader, buffers );
            break;
          case BufferSentinel.BufferLengths:
            ReadBufferLengths( reader, buffers );
            break;
          case BufferSentinel.BufferData:
            ReadBufferOffsets( reader, buffers );
            break;
          default:
            Fail( $"Unknown Buffer Sentinel: {sentinel:X}" );
            break;
        }

        Assert( reader.BaseStream.Position == endOffset,
          "Reader position does not match the buffer sentinel's end offset." );
      }

      Assert( reader.BaseStream.Position == sectionEndOffset,
          "Reader position does not match the buffer section's end offset." );
    }

    private void ReadBufferTypeInfo( BinaryReader reader, List<S3DGeometryBuffer> geometryBuffers )
    {
      foreach ( var buffer in geometryBuffers )
      {
        buffer.BufferInfo = new S3DGeometryBufferInfo
        {
          Unk_01 = reader.ReadUInt16(), // TODO
          Unk_02 = reader.ReadByte(), // TODO
          BufferType = ( S3DGeometryBufferType ) reader.ReadByte(),
          Unk_04 = reader.ReadByte(), // TODO
          Unk_05 = reader.ReadByte(), // TODO
          Unk_06 = reader.ReadByte(), // TODO
          Unk_07 = reader.ReadByte(), // TODO
          Unk_08 = reader.ReadByte(), // TODO
          Unk_09 = reader.ReadByte(), // TODO
        };
      }
    }

    private void ReadBufferElementSizes( BinaryReader reader, List<S3DGeometryBuffer> geometryBuffers )
    {
      foreach ( var buffer in geometryBuffers )
        buffer.ElementSize = reader.ReadUInt16();
    }

    private void ReadBufferLengths( BinaryReader reader, List<S3DGeometryBuffer> geometryBuffers )
    {
      foreach ( var buffer in geometryBuffers )
        buffer.BufferLength = reader.ReadUInt32();
    }

    private void ReadBufferOffsets( BinaryReader reader, List<S3DGeometryBuffer> geometryBuffers )
    {
      /* This section is the actual buffer data.
       * For now, we're just seeking and grabbing the start/end offsets.
       * We should parse this similar to M3DSpline in the future for ease of use.
       */

      foreach ( var buffer in geometryBuffers )
      {
        buffer.StartOffset = reader.BaseStream.Position;
        reader.BaseStream.Position += buffer.BufferLength;
        buffer.EndOffset = reader.BaseStream.Position;
      }
    }

    private enum BufferSentinel : ushort
    {
      BufferTypeInfo = 0x0000,
      BufferElementSizes = 0x0001,
      BufferLengths = 0x0002,
      BufferData = 0x0003
    }

  }

}
