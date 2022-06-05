namespace Saber3D.Files
{

  public interface IS3DArchiveFileEntry : IS3DFile
  {

    #region Properties

    IS3DArchiveFile ParentFile { get; }

    bool IsReference { get; }
    long Offset { get; }
    int SizeInBytes { get; }

    #endregion

  }

}
