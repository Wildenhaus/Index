namespace Saber3D.Common
{

  public static class NumericExtensions
  {

    public static float ConvertToSNormFloat( this short value )
      => value / ( float ) short.MaxValue;

  }

}
