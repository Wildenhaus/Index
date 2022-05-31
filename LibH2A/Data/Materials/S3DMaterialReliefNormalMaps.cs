namespace Saber3D.Data.Materials
{

  public class S3DMaterialReliefNormalMaps
  {

    [MaterialProperty( "macro" )]
    public S3DMaterialNormalMap Macro { get; set; }

    [MaterialProperty( "micro1" )]
    public S3DMaterialNormalMap Micro1 { get; set; }

    [MaterialProperty( "micro2" )]
    public S3DMaterialNormalMap Micro2 { get; set; }

  }

}
