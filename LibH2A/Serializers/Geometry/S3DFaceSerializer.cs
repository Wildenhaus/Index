using System.Collections.Generic;
using System.IO;
using Saber3D.Data;
using Saber3D.Data.Geometry;
using static Saber3D.Assertions;

namespace Saber3D.Serializers.Geometry
{

  public class S3DFaceSerializer : S3DGeometryElementSerializer
  {

    #region Constructor

    public S3DFaceSerializer( S3DGeometryBuffer buffer )
      : base( buffer )
    {
    }

    #endregion

    #region Public Methods

    public S3DFace Deserialize( BinaryReader reader )
    {
      // TODO: Is this correct (in terms of handling quads).
      var elementSize = Buffer.ElementSize;
      Assert( elementSize % 2 == 0,
        "Face data has an odd number of bytes. Something is very wrong." );

      var numIndices = elementSize / 2;
      var vertexIndices = new ushort[ numIndices ];
      for ( var i = 0; i < numIndices; i++ )
        vertexIndices[ i ] = reader.ReadUInt16();

      return S3DFace.Create( vertexIndices );
    }

    public IEnumerable<S3DFace> DeserializeRange( BinaryReader reader, int startIndex, int endIndex )
    {
      var startOffset = Buffer.StartOffset + ( startIndex * Buffer.ElementSize );
      var length = endIndex - startIndex;

      reader.BaseStream.Position = startOffset;

      for ( var i = 0; i < length; i++ )
        yield return Deserialize( reader );
    }

    #endregion

  }

}
