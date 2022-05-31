namespace Saber3D.Data.Materials
{

  public class S3DMaterialBlending
  {

    [MaterialProperty( "method" )]
    public string Method { get; set; }

    [MaterialProperty( "useLayerAlpha" )]
    public bool UseLayerAlpha { get; set; }

    [MaterialProperty( "useHeightmap" )]
    public bool UseHeightMap { get; set; }

    [MaterialProperty( "weightMultiplier" )]
    public float WeightMultiplier { get; set; }

    [MaterialProperty( "heightmapSoftness" )]
    public float HeightMapSoftness { get; set; }

    [MaterialProperty( "texChannelBlendMask" )]
    public int TexChannelBlendMask { get; set; }

    [MaterialProperty( "weights" )]
    public S3DMaterialWeights Weights { get; set; }

    [MaterialProperty( "heightmap" )]
    public S3DMaterialHeightMap HeightMap { get; set; }

    [MaterialProperty( "heightmapOverride" )]
    public string HeightMapOverride { get; set; }

    [MaterialProperty( "upVector" )]
    public S3DMaterialUpVector UpVector { get; set; }

    [MaterialProperty( "heightmapUVOverride" )]
    public S3DMaterialHeightMapUvOverride HeightMapUvOverride { get; set; }

  }

}
