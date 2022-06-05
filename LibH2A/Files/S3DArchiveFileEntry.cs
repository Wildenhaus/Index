using System.IO;

namespace Saber3D.Files
{

  public class S3DArchiveEntryFile : S3DFile, IS3DArchiveFileEntry
  {

    #region Data Members

    private IS3DArchiveFile _parentFile;
    private long _offset;
    private int _size;

    #endregion

    #region Properties

    public IS3DArchiveFile ParentFile
    {
      get => _parentFile;
    }

    public bool IsReference
    {
      get => _size == 0;
    }

    public long Offset
    {
      get => _offset;
    }

    public int SizeInBytes
    {
      get => _size;
    }

    #endregion

    #region Constructor

    internal S3DArchiveEntryFile( IS3DArchiveFile parentFile, string name, long offset, int size )
      : base( name, null, true )
    {
      _parentFile = parentFile;
      _offset = offset;
      _size = size;
    }

    #endregion

    #region Overrides

    public override Stream GetStream()
      => _parentFile.GetEntryStream( this );

    #endregion

  }

}
