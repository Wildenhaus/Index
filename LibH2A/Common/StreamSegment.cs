using System;
using System.IO;

namespace Saber3D.Common
{

  public class StreamSegment : Stream
  {

    #region Data Members

    private Stream _baseStream;

    private long _startOffset;
    private long _endOffset;
    private long _length;
    private long _position;

    #endregion

    #region Properties

    public override bool CanRead => _baseStream.CanRead;
    public override bool CanSeek => _baseStream.CanSeek;
    public override bool CanWrite => _baseStream.CanWrite;

    public override long Length => _length;
    public override long Position
    {
      get => _position;
      set => Seek( value, SeekOrigin.Begin );
    }

    #endregion

    #region Constructor

    public StreamSegment( Stream stream, long startOffset, long length )
    {
      _baseStream = stream;

      _startOffset = startOffset;
      _endOffset = _startOffset + length;
      _length = length;
      _position = 0;
    }

    #endregion

    #region Overrides

    public override void Flush()
      => _baseStream.Flush();

    public override int Read( byte[] buffer, int offset, int count )
    {
      var expectedPos = _startOffset + _position;

      lock ( _baseStream )
      {
        // Ensure we're at the proper position
        if ( _baseStream.Position != expectedPos )
          Seek( _position, SeekOrigin.Begin );

        // Prevent out-of-bounds
        long bytesToRead = offset + count;
        if ( _position + bytesToRead > _endOffset )
          bytesToRead = _endOffset - _position - offset;

        var bytesRead = _baseStream.Read( buffer, offset, ( int ) bytesToRead );
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

      lock ( _baseStream )
      {
        _baseStream.Seek( offset, SeekOrigin.Begin );
        _position = offset - _startOffset;

        return _position;
      }
    }

    public override void SetLength( long value )
    {
      throw new NotImplementedException();
    }

    public override void Write( byte[] buffer, int offset, int count )
    {
      throw new NotImplementedException();
    }

    #endregion

  }

}
