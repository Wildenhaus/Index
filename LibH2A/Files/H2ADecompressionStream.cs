using System;
using System.IO;
using Ionic.Zlib;
using static Saber3D.Assertions;

namespace Saber3D.Files
{

  public class H2ADecompressionStream : Stream
  {

    #region Constants

    private const long OFFSET_CHUNK_COUNT = 0x0;
    private const long OFFSET_COMPRESSION_TYPE = 0x4;
    private const long OFFSET_CHUNK_OFFSETS = 0x8;

    private const int HEADER_SIZE = 0x600000;
    private const int CHUNK_SIZE = 0x8000;
    private const int MAX_CHUNK_COUNT = ( HEADER_SIZE - sizeof( long ) ) / sizeof( long );

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

      Assert( offset >= 0, "Seek offset must be positive." );
      Assert( offset < _length, "Seek offset was out of bounds." );

      // Find the appropriate chunk.
      var position = 0l;
      var chunkIndex = 0;
      for ( var i = 0; i < _chunkCount; i++ )
      {
        var chunk = _chunks[ i ];
        if ( position + chunk.Length > offset )
          break;

        position += chunk.Length;
        chunkIndex++;
      }

      SetCurrentChunk( chunkIndex );

      // Set position within chunk
      var chunkPosition = offset - position;

      if ( _isCompressed )
        _decompressStream.Position = chunkPosition;
      else
        _baseStream.Position += chunkPosition;

      _position += chunkPosition;

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

      using ( var zlibStream = new ZlibStream( _baseStream, CompressionMode.Decompress, true ) )
      {
        _decompressStream.SetLength( CHUNK_SIZE );

        var bytesRead = zlibStream.Read( _decompressBuffer, 0, ( int ) chunk.Length );

        _decompressStream.Position = 0;
        _decompressStream.SetLength( bytesRead );
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

}
