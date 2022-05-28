using System.Runtime.CompilerServices;

namespace Saber3D.Data
{

  public abstract class M3DSpline
  {

    #region Data Members

    private readonly M3DSplineData _data;

    #endregion

    #region Properties

    public abstract SplineType Type { get; }

    public uint Count
    {
      [MethodImpl( MethodImplOptions.AggressiveInlining )]
      get => _data.Count;
    }

    #endregion

    #region Constructor

    protected M3DSpline( M3DSplineData data )
    {
      _data = data;
    }

    #endregion

  }

  public class M3DSplineLinear1D : M3DSpline
  {

    #region Properties

    public override SplineType Type => SplineType.Linear1D;

    #endregion

    #region Constructor

    public M3DSplineLinear1D( M3DSplineData splineData )
      : base( splineData )
    {
    }

    #endregion

  }

  public class M3DSplineLinear2D : M3DSpline
  {

    #region Properties

    public override SplineType Type => SplineType.Linear2D;

    #endregion

    #region Constructor

    public M3DSplineLinear2D( M3DSplineData splineData )
      : base( splineData )
    {
    }

    #endregion

  }

  public class M3DSplineLinear3D : M3DSpline
  {

    #region Properties

    public override SplineType Type => SplineType.Linear3D;

    #endregion

    #region Constructor

    public M3DSplineLinear3D( M3DSplineData splineData )
      : base( splineData )
    {
    }

    #endregion

  }

  public class M3DSplineHermit : M3DSpline
  {

    #region Properties

    public override SplineType Type => SplineType.Hermit;

    #endregion

    #region Constructor

    public M3DSplineHermit( M3DSplineData splineData )
      : base( splineData )
    {
    }

    #endregion

  }

  public class M3DSplineBezier2D : M3DSpline
  {

    #region Properties

    public override SplineType Type => SplineType.Bezier2D;

    #endregion

    #region Constructor

    public M3DSplineBezier2D( M3DSplineData splineData )
      : base( splineData )
    {
    }

    #endregion

  }

  public class M3DSplineBezier3D : M3DSpline
  {

    #region Properties

    public override SplineType Type => SplineType.Bezier3D;

    #endregion

    #region Constructor

    public M3DSplineBezier3D( M3DSplineData splineData )
      : base( splineData )
    {
    }

    #endregion

  }

  public class M3DSplineLagrange : M3DSpline
  {

    #region Properties

    public override SplineType Type => SplineType.Lagrange;

    #endregion

    #region Constructor

    public M3DSplineLagrange( M3DSplineData splineData )
      : base( splineData )
    {
    }

    #endregion

  }

  public class M3DSplineQuat : M3DSpline
  {

    #region Properties

    public override SplineType Type => SplineType.Quat;

    #endregion

    #region Constructor

    public M3DSplineQuat( M3DSplineData splineData )
      : base( splineData )
    {
    }

    #endregion

  }

  public class M3DSplineColor : M3DSpline
  {

    #region Properties

    public override SplineType Type => SplineType.Color;

    #endregion

    #region Constructor

    public M3DSplineColor( M3DSplineData splineData )
      : base( splineData )
    {
    }

    #endregion

  }

  public struct M3DSplineData
  {

    public SplineType SplineType;
    public byte CompressedDataSize;
    public byte Unk_02;
    public byte Unk_03;
    public uint Count;
    public uint SizeInBytes;
    public float[] Data;

  }

  public enum SplineType : byte
  {
    Linear1D = 0,
    Linear2D = 1,
    Linear3D = 2,
    Hermit = 3,
    Bezier2D = 4,
    Bezier3D = 5,
    Lagrange = 6,
    Quat = 7,
    Color = 8
  }

}
