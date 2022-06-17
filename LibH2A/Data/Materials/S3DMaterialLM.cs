using Saber3D.Data.Shared;

namespace Saber3D.Data.Materials
{

  public class S3DMaterialLM
  {

    [ConfigurationProperty( "source" )]
    public string Source { get; set; }

    [ConfigurationProperty( "texName" )]
    public string TextureName { get; set; }

    [ConfigurationProperty( "uvSetIdx" )]
    public int UvSetIndex { get; set; }

    [ConfigurationProperty( "tangent" )]
    public S3DMaterialTangent Tangent { get; set; }

  }

}
