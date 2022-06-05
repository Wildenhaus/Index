using System.IO;
using static Saber3D.Assertions;

namespace Saber3D.Files
{

  public abstract class S3DFile : IS3DFile
  {

    #region Constants

    protected const int SIGNATURE_SER = 0x52455331;

    #endregion

    #region Data Members

    private string _name;

    private H2AFileContext _fileContext;

    protected Stream _stream;
    protected BinaryReader _reader;

    private bool _isInitialized;
    private bool _isDisposed;
    private bool _shouldDisposeBaseStream;

    #endregion

    #region Properties

    public string Name
    {
      get => _name;
    }

    public H2AFileContext FileContext
    {
      get => _fileContext;
    }

    public bool IsDisposed
    {
      get => _isDisposed;
    }

    public bool IsInitialized
    {
      get => _isInitialized;
    }

    protected Stream BaseStream
    {
      get => _stream;
    }

    protected BinaryReader Reader
    {
      get => _reader;
    }

    #endregion

    #region Constructor

    protected S3DFile( string name, Stream stream, bool keepStreamOpen = false )
    {
      _name = name;
      _stream = stream;

      if ( _stream != null )
        _reader = new BinaryReader( stream );

      _shouldDisposeBaseStream = !keepStreamOpen;
    }

    ~S3DFile()
    {
      Dispose( false );
    }

    #endregion

    #region Public Methods

    public virtual Stream GetStream()
    {
      return _stream;
    }

    public void SetFileContext( H2AFileContext fileContext )
    {
      if ( _fileContext != null )
        _fileContext.RemoveFile( this );

      fileContext.AddFile( this );
      _fileContext = fileContext;
    }

    #endregion

    #region Private Methods

    protected void Initialize()
    {
      if ( _isInitialized )
        return;

      OnInitialize();

      _isInitialized = true;
    }

    protected virtual void OnInitialize()
    {
    }

    protected void CheckSignature( int fileSignature )
    {
      Assert( Reader.ReadInt32() == SIGNATURE_SER, "1SER signature not found." );
      Assert( Reader.ReadInt32() == fileSignature, "File signature not found." );
    }

    #endregion

    #region IDisposable Members

    public void Dispose()
      => Dispose( true );

    private void Dispose( bool isDisposing )
    {
      if ( _isDisposed )
        return;

      OnDisposing( isDisposing );

      if ( isDisposing )
      {
        _reader?.Dispose();
        if ( _shouldDisposeBaseStream )
          _stream?.Dispose();
      }

      _isDisposed = true;
    }

    protected virtual void OnDisposing( bool isDisposing )
    {
    }

    #endregion

  }

}
