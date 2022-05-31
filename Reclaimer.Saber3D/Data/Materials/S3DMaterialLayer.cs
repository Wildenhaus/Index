namespace Saber3D.Data.Materials
{

  public class S3DMaterialLayer
  {

    [MaterialProperty( "texName" )]
    public string TextureName { get; set; }

    [MaterialProperty( "mtlName" )]
    public string MaterialName { get; set; }

    [MaterialProperty( "tint" )]
    public float[] Tint { get; set; }

    [MaterialProperty( "vcSet" )]
    public int VcSet { get; set; }

    [MaterialProperty( "tilingU" )]
    public float TilingU { get; set; }

    [MaterialProperty( "tilingV" )]
    public float TilingV { get; set; }

    [MaterialProperty( "blending" )]
    public S3DMaterialBlending Blending { get; set; }

    [MaterialProperty( "uvSetIdx" )]
    public int UvSetIndex { get; set; }

  }

}
