namespace Saber3D.Data.Materials
{

  public class S3DMaterialHeightMapUvOverride
  {

    [MaterialProperty( "enabled" )]
    public bool Enabled { get; set; }

    [MaterialProperty( "tilingU" )]
    public float TilingU { get; set; }

    [MaterialProperty( "tilingV" )]
    public float TilingV { get; set; }

    [MaterialProperty( "uvSetIdx" )]
    public int UvSetIndex { get; set; }

  }

}
