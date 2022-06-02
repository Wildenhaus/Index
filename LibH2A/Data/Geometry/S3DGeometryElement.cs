namespace Saber3D.Data.Geometry
{

  public abstract class S3DGeometryElement
  {

    #region Data Members

    private uint _index;

    #endregion

    #region Properties

    public uint Index
    {
      get => _index;
    }

    public abstract S3DGeometryElementType ElementType { get; }

    #endregion

  }

}
