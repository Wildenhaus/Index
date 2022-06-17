using Saber3D.Data.Shared;

namespace Saber3D.Data.Materials
{

  public class S3DMaterialBlending
  {

    [ConfigurationProperty( "method" )]
    public string Method { get; set; }

    [ConfigurationProperty( "useLayerAlpha" )]
    public bool UseLayerAlpha { get; set; }

    [ConfigurationProperty( "useHeightmap" )]
    public bool UseHeightMap { get; set; }

    [ConfigurationProperty( "weightMultiplier" )]
    public float WeightMultiplier { get; set; }

    [ConfigurationProperty( "heightmapSoftness" )]
    public float HeightMapSoftness { get; set; }

    [ConfigurationProperty( "texChannelBlendMask" )]
    public int TexChannelBlendMask { get; set; }

    [ConfigurationProperty( "weights" )]
    public S3DMaterialWeights Weights { get; set; }

    [ConfigurationProperty( "heightmap" )]
    public S3DMaterialHeightMap HeightMap { get; set; }

    [ConfigurationProperty( "heightmapOverride" )]
    public string HeightMapOverride { get; set; }

    [ConfigurationProperty( "upVector" )]
    public S3DMaterialUpVector UpVector { get; set; }

    [ConfigurationProperty( "heightmapUVOverride" )]
    public S3DMaterialHeightMapUvOverride HeightMapUvOverride { get; set; }

  }

}
