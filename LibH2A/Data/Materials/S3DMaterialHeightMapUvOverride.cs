using Saber3D.Data.Shared;

namespace Saber3D.Data.Materials
{

  public class S3DMaterialHeightMapUvOverride
  {

    [ConfigurationProperty( "enabled" )]
    public bool Enabled { get; set; }

    [ConfigurationProperty( "tilingU" )]
    public float TilingU { get; set; }

    [ConfigurationProperty( "tilingV" )]
    public float TilingV { get; set; }

    [ConfigurationProperty( "uvSetIdx" )]
    public int UvSetIndex { get; set; }

  }

}
