using System.Collections.Generic;
using System.IO;

namespace Saber3D.Files
{

  public abstract class S3DFile : IS3DFile
  {

    #region Data Members

    private string _name;

    private IS3DFile _parent;
    private IList<IS3DFile> _children;
    private H2AFileContext _fileContext;

    private Stream _stream;
    private BinaryReader _reader;

    private bool _isInitialized;
    private bool _isDisposed;

    #endregion

    #region Properties

    public string Name
    {
      get => _name;
    }

    public long SizeInBytes
    {
      get => _stream.Length;
    }

    public IS3DFile Parent
    {
      get => _parent;
    }

    public IEnumerable<IS3DFile> Children
    {
      get => _children;
    }

    public H2AFileContext FileContext
    {
      get => _fileContext;
    }

    public abstract string FileTypeDisplay { get; }

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

    protected S3DFile( string name, Stream stream, IS3DFile parent = null )
    {
      _name = SanitizeName( name );

      _stream = stream;
      _reader = new BinaryReader( _stream, System.Text.Encoding.UTF8, true );

      _parent = parent;
      _children = new List<IS3DFile>();
    }

    ~S3DFile()
    {
      Dispose( false );
    }

    #endregion

    #region Public Methods

    public void Initialize()
    {
      if ( _isInitialized )
        return;

      OnInitialize();

      _isInitialized = true;
    }

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

    #region Protected Methods

    protected virtual void OnInitialize()
    {
    }

    protected void AddChild( IS3DFile file )
    {
      _children.Add( file );
    }

    #endregion

    #region Private Methods

    private static string SanitizeName( string path )
    {
      if ( path.Contains( ":" ) )
        path = path.Substring( path.IndexOf( ':' ) + 1 );
      if ( path.Contains( ">" ) )
        path = path.Substring( path.IndexOf( '>' ) + 1 );

      return Path.GetFileName( path );
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
        foreach ( var child in _children )
          child?.Dispose();

        _fileContext?.RemoveFile( this );
      }

      _isDisposed = true;
    }

    protected virtual void OnDisposing( bool isDisposing )
    {
    }

    #endregion

  }

}
