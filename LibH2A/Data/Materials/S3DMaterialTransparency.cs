using Saber3D.Data.Shared;

namespace Saber3D.Data.Materials
{

  public class S3DMaterialTransparency
  {

    [ConfigurationProperty( "colorSetIdx" )]
    public int ColorSetIndex { get; set; }

    [ConfigurationProperty( "enabled" )]
    public int Enabled { get; set; }

    [ConfigurationProperty( "multiplier" )]
    public float Multiplier { get; set; }

    [ConfigurationProperty( "sources" )]
    public int Sources { get; set; }

  }

}
