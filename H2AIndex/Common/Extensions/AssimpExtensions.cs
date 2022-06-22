namespace H2AIndex.Common.Extensions
{

  public static class AssimpExtensions
  {

    public static Assimp.Matrix4x4 ToAssimp( this System.Numerics.Matrix4x4 m, bool transpose = true )
    {
      if ( transpose )
        m = System.Numerics.Matrix4x4.Transpose( m );

      return new Assimp.Matrix4x4(
        m.M11, m.M12, m.M13, m.M14,
        m.M21, m.M22, m.M23, m.M24,
        m.M31, m.M32, m.M33, m.M34,
        m.M41, m.M42, m.M43, m.M44
        );
    }

    public static System.Numerics.Matrix4x4 ToNumerics( this Assimp.Matrix4x4 m, bool transpose = true )
    {
      if ( transpose )
        m.Transpose();

      return new System.Numerics.Matrix4x4(
        m.A1, m.A2, m.A3, m.A4,
        m.B1, m.B2, m.B3, m.B4,
        m.C1, m.C2, m.C3, m.C4,
        m.D1, m.D2, m.D3, m.D4 );
    }

    public static Assimp.Vector3D ToAssimp( this System.Numerics.Vector3 v )
    {
      return new Assimp.Vector3D( v.X, v.Y, v.Z );
    }

    public static Assimp.Vector3D ToAssimp3D( this System.Numerics.Vector4 v )
    {
      return new Assimp.Vector3D( v.X, v.Y, v.Z );
    }

  }

}
