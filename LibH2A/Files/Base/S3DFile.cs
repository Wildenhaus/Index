using System.Collections.Generic;
using System.IO;

namespace Saber3D.Files
{

  public abstract class S3DFile : IS3DFile
  {

    #region Data Members

    private IS3DFile _parent;
    private IList<IS3DFile> _children;

    private string _name;
    private string _extension;

    private H2AStream _baseStream;
    public long _dataStartOffset;
    public long _dataEndOffset;
    private BinaryReader _reader;

    private bool _isInitialized;
    private bool _isDisposed;

    #endregion

    #region Properties

    public IS3DFile Parent => _parent;
    public IEnumerable<IS3DFile> Children => _children;

    public string Name => _name;
    public string Extension => _extension;
    public long SizeInBytes
    {
      get => _dataEndOffset - _dataStartOffset;
    }

    public abstract string FileTypeDisplay { get; }

    protected H2AStream BaseStream => _baseStream;
    protected long DataStartOffset => _dataStartOffset;
    protected long DataEndOffset => _dataEndOffset;
    protected BinaryReader Reader => _reader;

    #endregion

    #region Constructor

    protected S3DFile( string name, H2AStream baseStream,
      long dataStartOffset, long dataEndOffset,
      IS3DFile parent = null )
    {
      _name = SanitizeName( name );
      _extension = Path.GetExtension( _name );

      _baseStream = baseStream;
      _dataStartOffset = dataStartOffset;
      _dataEndOffset = dataEndOffset;
      _reader = new BinaryReader( _baseStream, System.Text.Encoding.UTF8, true );

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

    public virtual H2AStream GetStream()
      => new H2AStreamSegment( _baseStream, _dataStartOffset, SizeInBytes );

    #endregion

    #region Protected Methods

    protected virtual void OnInitialize()
    {
    }

    protected void AddChild( IS3DFile file )
    {
      _children.Add( file );
    }

    protected virtual string SanitizeName( string path )
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
      }

      _isDisposed = true;
    }

    protected virtual void OnDisposing( bool isDisposing )
    {
    }

    #endregion

    #region IEquatable Methods

    public bool Equals( IS3DFile other )
    {
      return Name.Equals( other.Name );
    }

    #endregion

  }

}
