﻿using System.Numerics;

namespace Saber3D.Data.Geometry
{

  public abstract class S3DVertex : S3DGeometryElement
  {

    #region Properties

    public Vector4 Position { get; set; }

    public float X
    {
      get => Position.X;
    }

    public float Y
    {
      get => Position.Y;
    }

    public float Z
    {
      get => Position.Z;
    }

    public Vector4? Normal { get; set; }

    public override S3DGeometryElementType ElementType
    {
      get => S3DGeometryElementType.Vertex;
    }

    #endregion

  }

  public class S3DVertexStatic : S3DVertex
  {
  }

  public class S3DVertexSkinned : S3DVertex
  {

    #region Properties

    // Are these types correct?
    public float? Weight1 { get; set; }
    public float? Weight2 { get; set; }
    public float? Weight3 { get; set; }
    public float? Weight4 { get; set; }

    public byte Index1 { get; set; }
    public byte Index2 { get; set; }
    public byte Index3 { get; set; }
    public byte Index4 { get; set; }

    #endregion

  }

}
