using System.Numerics;

namespace Saber3D.Data.Geometry
{

  public class S3DInterleavedData : S3DGeometryElement
  {

    #region Properties

    public Vector4 Tangent0 { get; set; }
    public Vector4 Tangent1 { get; set; }
    public Vector4 Tangent2 { get; set; }
    public Vector4 Tangent3 { get; set; }
    public Vector4 Tangent4 { get; set; }

    public Vector4 Color0 { get; set; }
    public Vector4 Color1 { get; set; }
    public Vector4 Color2 { get; set; }
    public Vector4 Color3 { get; set; }
    public Vector4 Color4 { get; set; }

    /* These can be 4D, which is why I'm using a Vector4.
     * It might be worthwhile to add properties/flags/etc to
     * denote whether or not it's 4D.
     */
    public Vector4 UV0 { get; set; }
    public Vector4 UV1 { get; set; }
    public Vector4 UV2 { get; set; }
    public Vector4 UV3 { get; set; }
    public Vector4 UV4 { get; set; }
    public Vector4 UV5 { get; set; }

    public override S3DGeometryElementType ElementType
    {
      get => S3DGeometryElementType.Material;
    }

    #endregion

  }

}
