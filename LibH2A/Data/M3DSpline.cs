using System.Runtime.CompilerServices;

namespace Saber3D.Data
{

  /// <summary>
  ///   A base spline structure.
  /// </summary>
  public abstract class M3DSpline
  {
    /* This is the base class for splines.
     * Every type of spline is serialized the same, but the shape
     * of the data will be different based on the actual type.
     * 
     * Derived classes can implement the functionality to deliver
     * the appropriate data based on the internal spline data.
     */

    #region Data Members

    /// <summary>
    ///   The spline data.
    /// </summary>
    private readonly M3DSplineData _data;

    #endregion

    #region Properties

    /// <summary>
    ///   The spline type.
    /// </summary>
    public abstract SplineType Type { get; }

    /// <summary>
    ///   The number of elements in the spline.
    /// </summary>
    public uint Count
    {
      [MethodImpl( MethodImplOptions.AggressiveInlining )]
      get => _data.Count;
    }

    #endregion

    #region Constructor

    /// <summary>
    ///   Constructs a new <see cref="M3DSpline" />.
    /// </summary>
    /// <param name="data">
    ///   The spline data.
    /// </param>
    protected M3DSpline( M3DSplineData data )
    {
      _data = data;
    }

    #endregion

  }

  /// <summary>
  ///   A Linear 1D Spline structure.
  /// </summary>
  public class M3DSplineLinear1D : M3DSpline
  {

    // TODO: Implement data accessors.

    #region Properties

    /// <inheritdoc cref="M3DSpline.Type" />
    public override SplineType Type => SplineType.Linear1D;

    #endregion

    #region Constructor

    public M3DSplineLinear1D( M3DSplineData splineData )
      : base( splineData )
    {
    }

    #endregion

  }

  /// <summary>
  ///   A Linear 2D Spline structure.
  /// </summary>
  public class M3DSplineLinear2D : M3DSpline
  {

    // TODO: Implement data accessors.

    #region Properties

    /// <inheritdoc cref="M3DSpline.Type" />
    public override SplineType Type => SplineType.Linear2D;

    #endregion

    #region Constructor

    public M3DSplineLinear2D( M3DSplineData splineData )
      : base( splineData )
    {
    }

    #endregion

  }

  /// <summary>
  ///   A Linear 3D Spline structure.
  /// </summary>
  public class M3DSplineLinear3D : M3DSpline
  {

    // TODO: Implement data accessors.

    #region Properties

    /// <inheritdoc cref="M3DSpline.Type" />
    public override SplineType Type => SplineType.Linear3D;

    #endregion

    #region Constructor

    public M3DSplineLinear3D( M3DSplineData splineData )
      : base( splineData )
    {
    }

    #endregion

  }

  /// <summary>
  ///   A Hermit Spline structure.
  /// </summary>
  public class M3DSplineHermit : M3DSpline
  {

    // TODO: Implement data accessors.

    #region Properties

    /// <inheritdoc cref="M3DSpline.Type" />
    public override SplineType Type => SplineType.Hermit;

    #endregion

    #region Constructor

    public M3DSplineHermit( M3DSplineData splineData )
      : base( splineData )
    {
    }

    #endregion

  }

  /// <summary>
  ///   A 2D Bezier Spline structure.
  /// </summary>
  public class M3DSplineBezier2D : M3DSpline
  {

    // TODO: Implement data accessors.

    #region Properties

    /// <inheritdoc cref="M3DSpline.Type" />
    public override SplineType Type => SplineType.Bezier2D;

    #endregion

    #region Constructor

    public M3DSplineBezier2D( M3DSplineData splineData )
      : base( splineData )
    {
    }

    #endregion

  }

  /// <summary>
  ///   A 3D Bezier Spline structure.
  /// </summary>
  public class M3DSplineBezier3D : M3DSpline
  {

    #region Properties

    // TODO: Implement data accessors.

    /// <inheritdoc cref="M3DSpline.Type" />
    public override SplineType Type => SplineType.Bezier3D;

    #endregion

    #region Constructor

    public M3DSplineBezier3D( M3DSplineData splineData )
      : base( splineData )
    {
    }

    #endregion

  }

  /// <summary>
  ///   A Lagrange Spline structure.
  /// </summary>
  public class M3DSplineLagrange : M3DSpline
  {

    // TODO: Implement data accessors.

    #region Properties

    /// <inheritdoc cref="M3DSpline.Type" />
    public override SplineType Type => SplineType.Lagrange;

    #endregion

    #region Constructor

    public M3DSplineLagrange( M3DSplineData splineData )
      : base( splineData )
    {
    }

    #endregion

  }

  /// <summary>
  ///   A Quaternarion Spline structure.
  /// </summary>
  public class M3DSplineQuat : M3DSpline
  {

    // TODO: Implement data accessors.

    #region Properties

    /// <inheritdoc cref="M3DSpline.Type" />
    public override SplineType Type => SplineType.Quat;

    #endregion

    #region Constructor

    public M3DSplineQuat( M3DSplineData splineData )
      : base( splineData )
    {
    }

    #endregion

  }

  /// <summary>
  ///   A Color Spline structure.
  /// </summary>
  public class M3DSplineColor : M3DSpline
  {

    // TODO: Implement data accessors.

    #region Properties

    /// <inheritdoc cref="M3DSpline.Type" />
    public override SplineType Type => SplineType.Color;

    #endregion

    #region Constructor

    public M3DSplineColor( M3DSplineData splineData )
      : base( splineData )
    {
    }

    #endregion

  }

  /// <summary>
  ///   The base data structure for splines.
  /// </summary>
  public struct M3DSplineData
  {

    #region Data Members

    /// <summary>
    ///  The spline type.
    /// </summary>
    public SplineType SplineType;

    /// <summary>
    ///   The compressed data size of the spline's elements.
    /// </summary>
    public byte CompressedDataSize;

    // TODO: Appears to be a dimension
    public byte Unk_02;

    // TODO: Appears to be a dimension
    public byte Unk_03;

    /// <summary>
    ///   The number of elements in the spline data.
    /// </summary>
    public uint Count;

    /// <summary>
    ///   The size of the spline's data in bytes.
    /// </summary>
    public uint SizeInBytes;

    /// <summary>
    ///   The raw spline element data.
    /// </summary>
    public float[] Data;

    #endregion

  }

  /// <summary>
  ///   An enumeration for spline types.
  /// </summary>
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
