using System.Collections;
using System.Collections.Generic;
using static Saber3D.Assertions;

namespace Saber3D.Data.Geometry
{

  public abstract class S3DFace : S3DGeometryElement, IEnumerable<ushort>
  {

    #region Data Members

    protected ushort[] _vertexIndices;

    #endregion

    #region Properties

    public int FaceCount
    {
      get => _vertexIndices.Length;
    }

    public override S3DGeometryElementType ElementType
    {
      get => S3DGeometryElementType.Face;
    }

    public ushort this[ int index ]
    {
      get => _vertexIndices[ index ];
      set => _vertexIndices[ index ] = value;
    }

    #endregion

    #region Constructor

    protected S3DFace( ushort[] vertexIndices )
    {
      _vertexIndices = vertexIndices;
    }

    public static S3DFace Create( ushort[] vertexIndices )
    {
      Assert( vertexIndices.Length > 1,
        "An S3DFace must have at least 2 vertices." );

      switch ( vertexIndices.Length )
      {
        case 3:
          return new S3DFaceTri( vertexIndices );
        case 4:
          return new S3DFaceQuad( vertexIndices );
        default:
          return new S3DFaceNgon( vertexIndices );
      }
    }

    #endregion

    #region IEnumerable Methods

    public IEnumerator<ushort> GetEnumerator()
    {
      foreach ( var index in _vertexIndices )
        yield return index;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      foreach ( var index in _vertexIndices )
        yield return index;
    }

    #endregion

  }

  public class S3DEdge : S3DFace
  {

    #region Properties

    public ushort VertexA
    {
      get => this[ 0 ];
      set => this[ 0 ] = value;
    }

    public ushort VertexB
    {
      get => this[ 1 ];
      set => this[ 1 ] = value;
    }

    #endregion

    #region Constructor

    public S3DEdge( ushort[] vertexIndices )
      : base( vertexIndices )
    {
      Assert( vertexIndices.Length == 2,
        "S3DEdge must have exactly 2 vertices." );
    }

    #endregion

  }

  public class S3DFaceTri : S3DFace
  {

    #region Properties

    public ushort VertexA
    {
      get => this[ 0 ];
      set => this[ 0 ] = value;
    }

    public ushort VertexB
    {
      get => this[ 1 ];
      set => this[ 1 ] = value;
    }

    public ushort VertexC
    {
      get => this[ 2 ];
      set => this[ 2 ] = value;
    }

    #endregion

    #region Constructor

    public S3DFaceTri( ushort[] vertexIndices )
      : base( vertexIndices )
    {
      Assert( vertexIndices.Length == 3,
        "S3DFaceTri must have exactly 3 vertices." );
    }

    #endregion

  }

  public class S3DFaceQuad : S3DFace
  {

    #region Properties

    public ushort VertexA
    {
      get => this[ 0 ];
      set => this[ 0 ] = value;
    }

    public ushort VertexB
    {
      get => this[ 1 ];
      set => this[ 1 ] = value;
    }

    public ushort VertexC
    {
      get => this[ 2 ];
      set => this[ 2 ] = value;
    }

    public ushort VertexD
    {
      get => this[ 3 ];
      set => this[ 3 ] = value;
    }

    #endregion

    #region Constructor

    public S3DFaceQuad( ushort[] vertexIndices )
      : base( vertexIndices )
    {
      Assert( vertexIndices.Length == 4,
        "S3DFaceQuad must have exactly 4 vertices." );
    }

    #endregion

  }

  public class S3DFaceNgon : S3DFace
  {

    #region Constructor

    public S3DFaceNgon( ushort[] vertexIndices )
      : base( vertexIndices )
    {
    }

    #endregion

  }

}
