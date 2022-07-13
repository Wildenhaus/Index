using System;
using System.IO;
using System.Threading;
using Ionic.Zlib;
using Saber3D.Common;
using static Saber3D.Assertions;

namespace Saber3D.Files
{

  /* H2A Decompression Stream
   * Original implementation by Zatarita
   * 
   * Pck files are stored as a series of chunks. Sometimes those chunks
   * are compressed with ZLib. In either case, this class provides an abstraction
   * to read these chunked files as if they were raw.
   */

  /// <summary>
  ///   The base class for H2A File Streams.
  ///   This incorporates locking for thread safety.
  /// </summary>
  public abstract class H2AStream : Stream
  {

    #region Data Members

    private readonly SemaphoreSlim _consumerLock;

    #endregion

    #region Properties

    public bool IsLocked { get; private set; }

    #endregion

    #region Constructor

    protected H2AStream()
    {
      _consumerLock = new SemaphoreSlim( 1, 1 );
    }

    ~H2AStream()
    {
      if ( IsLocked )
        ReleaseLock();

      _consumerLock?.Dispose();
    }

    #endregion

    #region Public Methods

    public void AcquireLock()
    {
      _consumerLock.Wait();

      IsLocked = true;
      OnLock();
    }

    public void ReleaseLock()
    {
      IsLocked = false;
      OnRelease();

      _consumerLock.Release();
    }

    #endregion

    #region Virtual Methods

    protected virtual void OnLock()
    {
    }

    protected virtual void OnRelease()
    {
    }

    #endregion

  }

  /// <summary>
  ///   A stream for raw files that have been extracted.
  ///   This class exists to fit into the thread safety paradigm.
  /// </summary>
  internal class H2AExtractedFileStream : H2AStream
  {

    #region Data Members

    private readonly Stream _baseStream;

    #endregion

    #region Properties

    public override bool CanRead => _baseStream.CanRead;
    public override bool CanSeek => _baseStream.CanSeek;
    public override bool CanWrite => _baseStream.CanWrite;
    public override long Length => _baseStream.Length;
    public override long Position
    {
      get => _baseStream.Position;
      set => _baseStream.Position = value;
    }

    #endregion

    #region Constructor

    private H2AExtractedFileStream( Stream baseStream )
    {
      _baseStream = baseStream;
    }

    public static H2AStream FromStream( Stream stream )
      => new H2AExtractedFileStream( stream );

    public static H2AStream FromFile( string filePath )
      => FromStream( File.OpenRead( filePath ) );

    #endregion

    #region Overrides

    public override void Flush()
      => _baseStream.Flush();

    public override int Read( byte[] buffer, int offset, int count )
      => _baseStream.Read( buffer, offset, count );

    public override long Seek( long offset, SeekOrigin origin )
      => _baseStream.Seek( offset, origin );

    public override void SetLength( long value )
      => _baseStream.SetLength( value );

    public override void Write( byte[] buffer, int offset, int count )
      => _baseStream.Write( buffer, offset, count );

    #endregion

  }

  /// <summary>
  ///   A stream that reads from compressed/chunked files such as pck.
  /// </summary>
  internal sealed class H2ADecompressionStream : H2AStream
  {

    #region Constants

    private const long OFFSET_CHUNK_COUNT = 0x0;
    private const long OFFSET_COMPRESSION_TYPE = 0x4;
    private const long OFFSET_CHUNK_OFFSETS = 0x8;

    private const long HEADER_SIZE = 0x600000;
    private const long CHUNK_SIZE = 0x8000;
    private const long MAX_CHUNK_COUNT = ( HEADER_SIZE - sizeof( long ) ) / sizeof( long );

    private const int CACHE_SIZE = 400;

    #endregion

    #region Data Members

    private Stream _baseStream;
    private BinaryReader _reader;

    private int _chunkCount;
    private bool _isCompressed;

    private long _length;
    private long _position;

    private Chunk[] _chunks;

    private int _currentChunkIndex;
    private byte[] _decompressBuffer;
    private MemoryStream _decompressStream;
    private LruCache<int, MemoryStream> _decompressCache;

    #endregion

    #region Properties

    public override bool CanRead => true;
    public override bool CanSeek => true;
    public override bool CanWrite => false;
    public override long Length => _length;
    public override long Position
    {
      get => _position;
      set => Seek( value, SeekOrigin.Begin );
    }

    private Chunk CurrentChunk
    {
      get => _chunks[ _currentChunkIndex ];
    }

    #endregion

    #region Constructor

    private H2ADecompressionStream( Stream baseStream )
    {
      _baseStream = baseStream;
      _reader = new BinaryReader( _baseStream );
      _decompressCache = new LruCache<int, MemoryStream>( CACHE_SIZE );
    }

    public static H2ADecompressionStream FromStream( Stream baseStream )
    {
      var stream = new H2ADecompressionStream( baseStream );
      stream.Initialize();

      return stream;
    }

    public static H2ADecompressionStream FromFile( string filePath )
      => H2ADecompressionStream.FromStream( File.OpenRead( filePath ) );

    #endregion

    #region Overrides

    public override int Read( byte[] buffer, int offset, int size )
    {
      if ( size == 0 )
        return -1;

      if ( _isCompressed )
        return ReadCompressed( buffer, offset, size );
      else
        return ReadUncompressed( buffer, offset, size );
    }

    public override long Seek( long offset, SeekOrigin origin )
    {
      // Calculate the true offset.
      switch ( origin )
      {
        case SeekOrigin.Begin:
          break;
        case SeekOrigin.End:
          offset = offset + _length;
          break;
        case SeekOrigin.Current:
          offset = offset + _position;
          break;
      }

      if ( offset == _position )
        return _position;

      Assert( offset >= 0, "Seek offset must be positive." );
      Assert( offset < _length, "Seek offset was out of bounds." );

      // Find the appropriate chunk.
      var chunkIndex = ( int ) ( offset / CHUNK_SIZE );
      SetCurrentChunk( chunkIndex );

      // Set position within chunk
      var chunkPosition = offset - _position;

      if ( _isCompressed )
        _decompressStream.Position = chunkPosition;
      else
        _baseStream.Position += chunkPosition;

      _position += chunkPosition;

      Assert( _position == offset );
      return _position;
    }

    public override void Write( byte[] buffer, int offset, int count )
    {
      Fail( "Stream does not support modification." );
    }

    public override void SetLength( long value )
    {
      Fail( "Stream does not support modification." );
    }

    public override void Flush()
    {
      Fail( "Stream does not support modification." );
    }

    #endregion

    #region Private Methods

    #region Initialize Methods

    private void Initialize()
    {
      Assert( _baseStream.CanRead, "Base Stream is not readable." );
      Assert( _baseStream.CanSeek, "Base Stream is not seekable." );

      ReadChunkCount();
      ReadCompressionType();
      ReadChunkOffsets();

      _position = 0;
      SetCurrentChunk( 0 );
    }

    private void ReadChunkCount()
    {
      _baseStream.Seek( OFFSET_CHUNK_COUNT, SeekOrigin.Begin );

      _chunkCount = _reader.ReadInt32();
      Assert( _chunkCount <= MAX_CHUNK_COUNT, "File exceeds the maximum number of chunks." );
    }

    private void ReadCompressionType()
    {
      _baseStream.Seek( OFFSET_COMPRESSION_TYPE, SeekOrigin.Begin );

      var compressionType = _reader.ReadInt32();
      switch ( compressionType )
      {
        case 0x0:
          _isCompressed = true;
          _decompressBuffer = new byte[ CHUNK_SIZE ];
          _decompressStream = new MemoryStream( _decompressBuffer );
          break;
        case 0x4:
          _isCompressed = false;
          break;
        default:
          Fail( $"Unknown compression type: 0x{compressionType:X}" );
          break;
      }
    }

    private void ReadChunkOffsets()
    {
      _baseStream.Seek( OFFSET_CHUNK_OFFSETS, SeekOrigin.Begin );

      // Create chunks
      var chunkCount = _chunkCount;
      var chunks = _chunks = new Chunk[ chunkCount ];

      // Read chunks
      var streamSize = _baseStream.Length;
      for ( var i = 0; i < chunkCount; i++ )
      {
        var startOffset = _reader.ReadInt64();
        var endOffset = Math.Min( startOffset + CHUNK_SIZE, streamSize );

        if ( _isCompressed )
          chunks[ i ] = new Chunk( startOffset, endOffset, CHUNK_SIZE );
        else
          chunks[ i ] = new Chunk( startOffset, endOffset );
      }

      // If the stream is compressed, we need to obtain the uncompressed
      // size of the final chunk.
      if ( _isCompressed )
      {
        var lastChunkIndex = _chunkCount - 1;
        var lastChunk = chunks[ lastChunkIndex ];

        SetCurrentChunk( _chunkCount - 1 );

        lastChunk = new Chunk( lastChunk.StartOffset, lastChunk.EndOffset, _decompressStream.Length );
        chunks[ lastChunkIndex ] = lastChunk;
      }

      // Calculate stream length
      _length = 0;
      foreach ( var chunk in chunks )
        _length += chunk.Length;
    }

    #endregion

    #region Read Methods

    private void IncrementCurrentChunk()
      => SetCurrentChunk( _currentChunkIndex + 1 );

    private void SetCurrentChunk( int chunkIndex )
    {
      Assert( chunkIndex <= _chunkCount );

      _baseStream.Position = _chunks[ chunkIndex ].StartOffset;
      _currentChunkIndex = chunkIndex;

      if ( _isCompressed )
        DecompressChunk( chunkIndex );

      _position = chunkIndex * CHUNK_SIZE;
    }

    private int ReadCompressed( byte[] buffer, int offset, int size )
    {
      var bytesRead = 0;

      while ( bytesRead < size )
      {
        if ( _position == _length )
          return bytesRead;

        var remainingChunkBytes = CurrentChunk.Length - _decompressStream.Position;
        if ( remainingChunkBytes <= 0 )
        {
          IncrementCurrentChunk();
          remainingChunkBytes = CurrentChunk.Length - _decompressStream.Position;
        }

        var bytesToRead = ( int ) Math.Min( size - bytesRead, remainingChunkBytes );

        _decompressStream.Read( buffer, bytesRead, bytesToRead );
        bytesRead += bytesToRead;
        _position += bytesToRead;
      }

      return bytesRead;
    }

    private int ReadUncompressed( byte[] buffer, int offset, int size )
    {
      var bytesRead = 0;

      while ( bytesRead < size )
      {
        if ( _position == _length )
          return bytesRead;

        var remainingChunkBytes = CurrentChunk.EndOffset - _baseStream.Position;
        if ( remainingChunkBytes <= 0 )
        {
          IncrementCurrentChunk();
          remainingChunkBytes = CurrentChunk.EndOffset - _baseStream.Position;
        }

        var bytesToRead = ( int ) Math.Min( size - bytesRead, remainingChunkBytes );
        _baseStream.Read( buffer, bytesRead, bytesToRead );
        bytesRead += bytesToRead;
        _position += bytesToRead;
      }

      return bytesRead;
    }

    private void DecompressChunk( int chunkIndex )
    {
      var chunk = _chunks[ chunkIndex ];
      _baseStream.Position = chunk.StartOffset;

      if ( _decompressCache.TryGet( chunkIndex, out var cachedStream ) )
      {
        _decompressStream = cachedStream;
        _decompressStream.Position = 0;
        return;
      }

      using ( var zlibStream = new ZlibStream( _baseStream, CompressionMode.Decompress, true ) )
      {
        _decompressStream.SetLength( CHUNK_SIZE );

        var bytesRead = zlibStream.Read( _decompressBuffer, 0, ( int ) chunk.Length );

        _decompressStream.Position = 0;
        _decompressStream.SetLength( bytesRead );
        _decompressCache.Put( chunkIndex, _decompressStream );
      }
    }

    #endregion

    #endregion

    #region Embedded Types

    private readonly struct Chunk
    {

      public readonly long StartOffset;
      public readonly long EndOffset;
      public readonly long Length;

      public Chunk( long startOffset, long endOffset )
      {
        StartOffset = startOffset;
        EndOffset = endOffset;
        Length = endOffset - startOffset;
      }

      public Chunk( long startOffset, long endOffset, long uncompressedLength )
      {
        StartOffset = startOffset;
        EndOffset = endOffset;
        Length = uncompressedLength;
      }

    }

    #endregion

  }

  /// <summary>
  ///   A stream that represents a segment of a base stream.
  ///   This is also used as a wrapper so that multiple file handles
  ///   can have their own stream.
  /// </summary>
  internal sealed class H2AStreamSegment : H2AStream
  {

    #region Data Members

    private readonly H2AStream _decompressionStream;

    private long _startOffset;
    private long _endOffset;
    private long _length;
    private long _position;

    #endregion

    #region Properties

    public override bool CanRead => _decompressionStream.CanRead;
    public override bool CanSeek => _decompressionStream.CanSeek;
    public override bool CanWrite => _decompressionStream.CanWrite;

    public override long Length => _length;
    public override long Position
    {
      get => _position;
      set
      {
        var newPosition = value;
        Assert( newPosition >= 0, "Position cannot be negative." );
        Assert( newPosition < _length, "Position is out of bounds." );

        _position = newPosition;
        _decompressionStream.Position = _startOffset + newPosition;
      }
    }

    #endregion

    #region Constructor

    public H2AStreamSegment( H2AStream stream, long startOffset, long length )
    {
      _decompressionStream = stream;

      _startOffset = startOffset;
      _endOffset = _startOffset + length;
      _length = length;
      _position = 0;
    }

    #endregion

    #region Overrides

    public override int Read( byte[] buffer, int offset, int count )
    {
      var expectedPos = _startOffset + _position;

      lock ( _decompressionStream )
      {
        // Ensure we're at the proper position
        if ( _decompressionStream.Position != expectedPos )
          Seek( _position, SeekOrigin.Begin );

        // Prevent out-of-bounds
        long bytesToRead = offset + count;
        if ( _position + bytesToRead > _endOffset )
          bytesToRead = _endOffset - _position - offset;

        if ( bytesToRead > _length - _position )
          bytesToRead = Math.Max( 0, _length - _position );

        if ( bytesToRead <= 0 )
          return 0;

        var bytesRead = _decompressionStream.Read( buffer, offset, ( int ) bytesToRead );
        _position += bytesRead;
        return bytesRead;
      }
    }

    public override long Seek( long offset, SeekOrigin origin )
    {
      switch ( origin )
      {
        case SeekOrigin.Begin:
          offset = offset + _startOffset;
          break;
        case SeekOrigin.End:
          offset = offset + _endOffset;
          break;
        case SeekOrigin.Current:
          offset = offset + _position;
          break;
      }

      offset = Math.Max( _startOffset, offset );
      offset = Math.Min( _endOffset, offset );

      _decompressionStream.Seek( offset, SeekOrigin.Begin );
      _position = offset - _startOffset;

      return _position;
    }

    public override void Flush()
      => _decompressionStream.Flush();

    public override void SetLength( long value )
      => _decompressionStream.SetLength( value );

    public override void Write( byte[] buffer, int offset, int count )
      => _decompressionStream.Write( buffer, offset, count );

    protected override void OnLock()
      => _decompressionStream.AcquireLock();

    protected override void OnRelease()
      => _decompressionStream.ReleaseLock();

    #endregion

  }

}
