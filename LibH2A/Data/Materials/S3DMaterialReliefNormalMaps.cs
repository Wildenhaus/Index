using Saber3D.Data.Shared;

namespace Saber3D.Data.Materials
{

  public class S3DMaterialReliefNormalMaps
  {

    [ConfigurationProperty( "macro" )]
    public S3DMaterialNormalMap Macro { get; set; }

    [ConfigurationProperty( "micro1" )]
    public S3DMaterialNormalMap Micro1 { get; set; }

    [ConfigurationProperty( "micro2" )]
    public S3DMaterialNormalMap Micro2 { get; set; }

  }

}
