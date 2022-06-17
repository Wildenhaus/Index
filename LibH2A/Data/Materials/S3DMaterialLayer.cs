using Saber3D.Data.Shared;

namespace Saber3D.Data.Materials
{

  public class S3DMaterialLayer
  {

    [ConfigurationProperty( "texName" )]
    public string TextureName { get; set; }

    [ConfigurationProperty( "mtlName" )]
    public string MaterialName { get; set; }

    [ConfigurationProperty( "tint" )]
    public float[] Tint { get; set; }

    [ConfigurationProperty( "vcSet" )]
    public int VcSet { get; set; }

    [ConfigurationProperty( "tilingU" )]
    public float TilingU { get; set; }

    [ConfigurationProperty( "tilingV" )]
    public float TilingV { get; set; }

    [ConfigurationProperty( "blending" )]
    public S3DMaterialBlending Blending { get; set; }

    [ConfigurationProperty( "uvSetIdx" )]
    public int UvSetIndex { get; set; }

  }

}
