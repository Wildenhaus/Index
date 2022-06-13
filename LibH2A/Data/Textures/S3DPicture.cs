namespace Saber3D.Data.Textures
{

  public class S3DPicture
  {

    #region Properties

    public int Width { get; set; }
    public int Height { get; set; }
    public int Depth { get; set; }
    public int Faces { get; set; }
    public int MipMapCount { get; set; }
    public S3DPictureFormat Format { get; set; }

    public byte[] Data { get; set; }

    #endregion

  }

}
