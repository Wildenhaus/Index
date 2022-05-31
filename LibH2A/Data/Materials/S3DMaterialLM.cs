namespace Saber3D.Data.Materials
{

  public class S3DMaterialLM
  {

    [MaterialProperty( "source" )]
    public string Source { get; set; }

    [MaterialProperty( "texName" )]
    public string TextureName { get; set; }

    [MaterialProperty( "uvSetIdx" )]
    public int UvSetIndex { get; set; }

    [MaterialProperty( "tangent" )]
    public S3DMaterialTangent Tangent { get; set; }

  }

}
