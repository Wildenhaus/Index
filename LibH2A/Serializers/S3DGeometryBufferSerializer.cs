using System;
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
          case BufferSentinel.BufferFlags:
            ReadBufferFlags( reader, buffers );
            break;
          case BufferSentinel.BufferElementSizes:
            ReadBufferElementSizes( reader, buffers );
            break;
          case BufferSentinel.BufferLengths:
            ReadBufferLengths( reader, buffers );
            break;
          case BufferSentinel.BufferData:
            ReadBufferData( reader, buffers );
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

    private void ReadBufferFlags( BinaryReader reader, List<S3DGeometryBuffer> geometryBuffers )
    {
      foreach ( var buffer in geometryBuffers )
      {
        /* The data stored in each buffer is defined by a set of flags.
         * This is usually 0x3B or 0x3F in length.
         * It appears that it's always stored as a 64-bit int.
         */

        var flagCount = buffer.FlagSize = reader.ReadUInt16();
        var flagDataLength = Math.Ceiling( flagCount / 8f );
        Assert( flagDataLength == sizeof( ulong ), "GeometryBuffer flag data is not 8 bytes." );

        buffer.Flags = ( S3DGeometryBufferFlags ) reader.ReadUInt64();
      }
    }

    private void ReadBufferElementSizes( BinaryReader reader, List<S3DGeometryBuffer> geometryBuffers )
    {
      /* The size of each element in the buffer (stride length).
       * We can use this to calculate offsets and catch cases where we under/overread each element.
       */

      foreach ( var buffer in geometryBuffers )
        buffer.ElementSize = reader.ReadUInt16();
    }

    private void ReadBufferLengths( BinaryReader reader, List<S3DGeometryBuffer> geometryBuffers )
    {
      /* The total length (in bytes) of the buffer.
       */

      foreach ( var buffer in geometryBuffers )
        buffer.BufferLength = reader.ReadUInt32();
    }

    private void ReadBufferData( BinaryReader reader, List<S3DGeometryBuffer> geometryBuffers )
    {
      /* This is the actual buffer data. We're using the previously obtained length
       * to determine the start and end offsets.
       * 
       * We can also optionally deserialize the buffer elements here.
       */

      foreach ( var buffer in geometryBuffers )
      {
        buffer.StartOffset = reader.BaseStream.Position;
        buffer.EndOffset = buffer.StartOffset + buffer.BufferLength;

        reader.BaseStream.Position = buffer.EndOffset;
        //buffer.Elements = S3DGeometryElementSerializer.Deserialize( reader, buffer );
      }
    }

    private enum BufferSentinel : ushort
    {
      BufferFlags = 0x0000,
      BufferElementSizes = 0x0001,
      BufferLengths = 0x0002,
      BufferData = 0x0003
    }

  }

}
