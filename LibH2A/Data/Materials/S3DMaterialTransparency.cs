namespace Saber3D.Data.Materials
{

  public class S3DMaterialTransparency
  {

    [MaterialProperty( "colorSetIdx" )]
    public int ColorSetIndex { get; set; }

    [MaterialProperty( "enabled" )]
    public int Enabled { get; set; }

    [MaterialProperty( "multiplier" )]
    public float Multiplier { get; set; }

    [MaterialProperty( "sources" )]
    public int Sources { get; set; }

  }

}
