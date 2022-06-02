using Saber3D.Data.Geometry;

namespace Saber3D.Data
{

  public class S3DGeometryBuffer
  {

    public S3DGeometryBufferFlags Flags { get; set; }
    public ushort ElementSize { get; set; }
    public uint BufferLength { get; set; }

    public long StartOffset { get; set; }
    public long EndOffset { get; set; }

    public S3DGeometryElementType ElementType
    {
      get
      {
        if ( Elements is null || Elements.Length == 0 )
          return S3DGeometryElementType.Unknown;

        return Elements[ 0 ].ElementType;
      }
    }

    public S3DGeometryElement[] Elements { get; set; }

    public int Count
    {
      get => ( int ) ( ( EndOffset - StartOffset ) / ElementSize );
    }

  }

}