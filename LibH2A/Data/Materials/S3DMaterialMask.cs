namespace Saber3D.Data.Materials
{

  public class S3DMaterialMask
  {

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
