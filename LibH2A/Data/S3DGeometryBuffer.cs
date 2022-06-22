using System.Diagnostics;
using Saber3D.Data.Geometry;

namespace Saber3D.Data
{

  [DebuggerDisplay( "Buffer<{ElementType},{ElementSize}>[{Count}]" )]
  public class S3DGeometryBuffer
  {

    public ushort FlagSize { get; set; } // DEBUG
    public S3DGeometryBufferFlags Flags { get; set; }
    public ushort ElementSize { get; set; }
    public uint BufferLength { get; set; }

    public long StartOffset { get; set; }
    public long EndOffset { get; set; }

    public S3DGeometryElementType ElementType
    {
      get
      {
        if ( Flags.HasFlag( S3DGeometryBufferFlags._VERT ) )
          return S3DGeometryElementType.Vertex;

        if ( Flags.HasFlag( S3DGeometryBufferFlags._TANG0 )
          || Flags.HasFlag( S3DGeometryBufferFlags._TANG1 )
          || Flags.HasFlag( S3DGeometryBufferFlags._TANG2 )
          || Flags.HasFlag( S3DGeometryBufferFlags._TANG3 )
          || Flags.HasFlag( S3DGeometryBufferFlags._TEX0 )
          || Flags.HasFlag( S3DGeometryBufferFlags._TEX1 )
          || Flags.HasFlag( S3DGeometryBufferFlags._TEX2 )
          || Flags.HasFlag( S3DGeometryBufferFlags._TEX3 )
          || Flags.HasFlag( S3DGeometryBufferFlags._TEX4 )
          )
          return S3DGeometryElementType.Interleaved;

        if ( Flags == S3DGeometryBufferFlags._FACE ) // TODO: Is there a better way to detect this?
          return S3DGeometryElementType.Face;

        if ( Flags == S3DGeometryBufferFlags._BONE )
          return S3DGeometryElementType.BoneId;

        return S3DGeometryElementType.Unknown;
      }
    }

    public int Count
    {
      get => ( int ) ( ( EndOffset - StartOffset ) / ElementSize );
    }

  }

}