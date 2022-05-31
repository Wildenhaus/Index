namespace Saber3D.Data.Materials
{

  public class S3DMaterialHeightMap
  {

    [MaterialProperty( "colorSetIdx" )]
    public int ColorSetIndex { get; set; }

    [MaterialProperty( "invert" )]
    public bool Invert { get; set; }

  }

}
