using Saber3D.Data.Shared;

namespace Saber3D.Data.Materials
{

  public class S3DMaterialUpVector
  {

    [ConfigurationProperty( "angle" )]
    public float Angle { get; set; }

    [ConfigurationProperty( "enabled" )]
    public bool Enabled { get; set; }

    [ConfigurationProperty( "falloff" )]
    public float Falloff { get; set; }

  }

}
