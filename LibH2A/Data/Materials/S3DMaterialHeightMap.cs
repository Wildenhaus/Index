using Saber3D.Data.Shared;

namespace Saber3D.Data.Materials
{

  public class S3DMaterialHeightMap
  {

    [ConfigurationProperty( "colorSetIdx" )]
    public int ColorSetIndex { get; set; }

    [ConfigurationProperty( "invert" )]
    public bool Invert { get; set; }

  }

}
