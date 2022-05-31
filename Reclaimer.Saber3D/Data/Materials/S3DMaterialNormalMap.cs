namespace Saber3D.Data.Materials
{

  public class S3DMaterialNormalMap
  {

    [MaterialProperty( "end" )]
    public float End { get; set; }

    [MaterialProperty( "falloff" )]
    public float Falloff { get; set; }

    [MaterialProperty( "isVisible" )]
    public int IsVisible { get; set; }

    [MaterialProperty( "scale" )]
    public float Scale { get; set; }

    [MaterialProperty( "start" )]
    public float Start { get; set; }

    [MaterialProperty( "textureName" )]
    public string TextureName { get; set; }

    [MaterialProperty( "tilingU" )]
    public float TilingU { get; set; }

    [MaterialProperty( "tilingV" )]
    public float TilingV { get; set; }

    [MaterialProperty( "uvSetIdx" )]
    public int UvSetIndex { get; set; }

  }

}
