using Saber3D.Data.Shared;

namespace Saber3D.Data.Materials
{

  public class S3DMaterialExtraVertexColorData
  {

    [ConfigurationProperty( "colorA" )]
    public S3DMaterialColor ColorA { get; set; }

    [ConfigurationProperty( "colorB" )]
    public S3DMaterialColor ColorB { get; set; }

    [ConfigurationProperty( "colorG" )]
    public S3DMaterialColor ColorG { get; set; }

    [ConfigurationProperty( "colorR" )]
    public S3DMaterialColor ColorR { get; set; }

  }

}
