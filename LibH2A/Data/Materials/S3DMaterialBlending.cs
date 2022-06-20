using Saber3D.Data.Scripting;

namespace Saber3D.Data.Materials
{

  public class S3DMaterialBlending
  {

    [ScriptingProperty( "method" )]
    public string Method { get; set; }

    [ScriptingProperty( "useLayerAlpha" )]
    public bool UseLayerAlpha { get; set; }

    [ScriptingProperty( "useHeightmap" )]
    public bool UseHeightMap { get; set; }

    [ScriptingProperty( "weightMultiplier" )]
    public float WeightMultiplier { get; set; }

    [ScriptingProperty( "heightmapSoftness" )]
    public float HeightMapSoftness { get; set; }

    [ScriptingProperty( "texChannelBlendMask" )]
    public int TexChannelBlendMask { get; set; }

    [ScriptingProperty( "weights" )]
    public S3DMaterialWeights Weights { get; set; }

    [ScriptingProperty( "heightmap" )]
    public S3DMaterialHeightMap HeightMap { get; set; }

    [ScriptingProperty( "heightmapOverride" )]
    public string HeightMapOverride { get; set; }

    [ScriptingProperty( "upVector" )]
    public S3DMaterialUpVector UpVector { get; set; }

    [ScriptingProperty( "heightmapUVOverride" )]
    public S3DMaterialHeightMapUvOverride HeightMapUvOverride { get; set; }

  }

}
