using Saber3D.Data.Shared;

namespace Saber3D.Data.Materials
{

  public class S3DMaterialNormalMap
  {

    [ConfigurationProperty( "end" )]
    public float End { get; set; }

    [ConfigurationProperty( "falloff" )]
    public float Falloff { get; set; }

    [ConfigurationProperty( "isVisible" )]
    public int IsVisible { get; set; }

    [ConfigurationProperty( "scale" )]
    public float Scale { get; set; }

    [ConfigurationProperty( "start" )]
    public float Start { get; set; }

    [ConfigurationProperty( "textureName" )]
    public string TextureName { get; set; }

    [ConfigurationProperty( "tilingU" )]
    public float TilingU { get; set; }

    [ConfigurationProperty( "tilingV" )]
    public float TilingV { get; set; }

    [ConfigurationProperty( "uvSetIdx" )]
    public int UvSetIndex { get; set; }

  }

}
