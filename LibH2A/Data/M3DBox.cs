using System.Numerics;

namespace Saber3D.Data
{

  /// <summary>
  ///   A 3D box structure.
  /// </summary>
  public struct M3DBox
  {

    #region Data Members

    /// <summary>
    ///   The starting vertex.
    /// </summary>
    public Vector3 Low;

    /// <summary>
    ///   The ending vertex.
    /// </summary>
    public Vector3 High;

    #endregion

    #region Constructor

    /// <summary>
    ///   Constructs a new <see cref="M3DBox" />.
    /// </summary>
    /// <param name="low">
    ///   The starting vertex.
    /// </param>
    /// <param name="high">
    ///   The ending vertex.
    /// </param>
    public M3DBox( Vector3 low, Vector3 high )
    {
      Low = low;
      High = high;
    }

    #endregion

  }

}
