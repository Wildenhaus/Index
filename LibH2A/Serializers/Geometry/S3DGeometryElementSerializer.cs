using System;
using System.IO;
using Saber3D.Data;
using Saber3D.Data.Geometry;
using static Saber3D.Assertions;

namespace Saber3D.Serializers.Geometry
{

  public abstract class S3DGeometryElementSerializer
  {

    #region Properties

    protected S3DGeometryBuffer Buffer { get; }

    protected S3DGeometryBufferFlags Flags
    {
      get => Buffer.Flags;
    }

    #endregion

    #region Constructor

    protected S3DGeometryElementSerializer( S3DGeometryBuffer buffer )
    {
      Buffer = buffer;
    }

    #endregion

    #region Public Methods

    public static S3DGeometryElement[] Deserialize( BinaryReader reader, S3DGeometryBuffer buffer )
    {
      switch ( ResolveElementType( buffer ) )
      {
        case S3DGeometryElementType.Face:
        {
          var serializer = new S3DFaceSerializer( buffer );
          return DeserializeElements( reader, buffer, serializer.Deserialize );
        }
        case S3DGeometryElementType.Vertex:
        {
          var serializer = new S3DVertexSerializer( buffer );
          return DeserializeElements( reader, buffer, serializer.Deserialize );
        }
        case S3DGeometryElementType.Interleaved:
        {
          var serializer = new S3DInterleavedDataSerializer( buffer );
          return DeserializeElements( reader, buffer, serializer.Deserialize );
          return default; // TODO
        }
        default:
          reader.BaseStream.Position = buffer.EndOffset;
          return default;
          //return FailReturn<S3DGeometryElement[]>(
          //  "Unable to deserialize unknown buffer element." );
      }
    }

    #endregion

    #region Private Methods

    private static void EnsureReaderIsAtBufferStart( BinaryReader reader, S3DGeometryBuffer buffer )
    {
      if ( reader.BaseStream.Position != buffer.StartOffset )
        reader.BaseStream.Position = buffer.StartOffset;
    }

    private static S3DGeometryElementType ResolveElementType( S3DGeometryBuffer buffer )
    {
      var flags = buffer.Flags;

      if ( flags.HasFlag( S3DGeometryBufferFlags._VERT ) )
        return S3DGeometryElementType.Vertex;

      if ( flags.HasFlag( S3DGeometryBufferFlags._TANG0 )
        || flags.HasFlag( S3DGeometryBufferFlags._TANG1 )
        || flags.HasFlag( S3DGeometryBufferFlags._TANG2 )
        || flags.HasFlag( S3DGeometryBufferFlags._TANG3 )
        || flags.HasFlag( S3DGeometryBufferFlags._TEX0 )
        || flags.HasFlag( S3DGeometryBufferFlags._TEX1 )
        || flags.HasFlag( S3DGeometryBufferFlags._TEX2 )
        || flags.HasFlag( S3DGeometryBufferFlags._TEX3 )
        || flags.HasFlag( S3DGeometryBufferFlags._TEX4 )
        )
        return S3DGeometryElementType.Interleaved;

      if ( flags == S3DGeometryBufferFlags._FACE ) // TODO: Is there a better way to detect this?
        return S3DGeometryElementType.Face;
      else return S3DGeometryElementType.Unknown;
    }

    private static S3DGeometryElement[] DeserializeElements( BinaryReader reader, S3DGeometryBuffer buffer,
      Func<BinaryReader, S3DGeometryElement> deserializeDelegate )
    {
      EnsureReaderIsAtBufferStart( reader, buffer );

      var elementSize = buffer.ElementSize;
      var elementCount = buffer.Count;
      var elements = new S3DGeometryElement[ elementCount ];

      var lastPosition = reader.BaseStream.Position;
      for ( var i = 0; i < elementCount; i++ )
      {
        elements[ i ] = deserializeDelegate( reader );

        var bytesRead = reader.BaseStream.Position - lastPosition;
        if ( bytesRead > elementSize )
          Fail( $"Element over-read. Expected: {elementSize}, Actual: {bytesRead}" );
        else if ( bytesRead < elementSize )
          Fail( $"Element under-read. Expected: {elementSize}, Actual: {bytesRead}" );

        lastPosition = reader.BaseStream.Position;
      }

      Assert( reader.BaseStream.Position == buffer.EndOffset,
        "Read stream position does not match the buffer's end offset." );

      return elements;
    }

    #endregion

  }

}
