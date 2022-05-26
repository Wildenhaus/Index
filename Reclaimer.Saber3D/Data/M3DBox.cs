using System.Numerics;

namespace Saber3D.Data
{

  public struct M3DBox
  {

    public Vector3 Low;
    public Vector3 High;

    public M3DBox( Vector3 low, Vector3 high )
    {
      Low = low;
      High = high;
    }

  }

}
