using System;
using System.IO;
using Saber3D.Data.Textures;
using static Saber3D.Assertions;

namespace Saber3D.Serializers
{

  public class S3DPictureSerializer : SerializerBase<S3DPicture>
  {

    #region Constants

    private const int SIGNATURE_PICT = 0x50494354; // TCIP

    #endregion

    #region Overrides

    protected override void OnDeserialize( BinaryReader reader, S3DPicture pict )
    {
      while ( reader.BaseStream.Position < reader.BaseStream.Length )
      {
        var sentinel = ( PictureSentinel ) reader.ReadUInt16();
        var endOffset = reader.ReadUInt32();

        switch ( sentinel )
        {
          case PictureSentinel.Header:
            ReadHeader( reader, pict );
            break;
          case PictureSentinel.Dimensions:
            ReadDimensions( reader, pict );
            break;
          case PictureSentinel.Format:
            ReadFormat( reader, pict );
            break;
          case PictureSentinel.MipMaps:
            ReadMipMaps( reader, pict );
            break;
          case PictureSentinel.Data:
            ReadData( reader, pict, endOffset );
            break;
          case PictureSentinel.Footer:
            break;
          default:
            Fail( $"Unknown Picture Sentinel: {sentinel:X}" );
            break;
        }

        Assert( reader.BaseStream.Position == endOffset,
          "Reader position does not match the picture sentinel's end offset." );
      }
    }

    #endregion

    private void ReadHeader( BinaryReader reader, S3DPicture pict )
    {
      Assert( reader.ReadInt32() == SIGNATURE_PICT, "Invalid PICT signature." );
    }

    private void ReadDimensions( BinaryReader reader, S3DPicture pict )
    {
      pict.Width = reader.ReadInt32();
      pict.Height = reader.ReadInt32();
      pict.Depth = reader.ReadInt32();
      pict.Faces = reader.ReadInt32();
    }

    private void ReadFormat( BinaryReader reader, S3DPicture pict )
    {
      var formatValue = reader.ReadInt32();
      var format = ( S3DPictureFormat ) formatValue;

      Assert( Enum.IsDefined( typeof( S3DPictureFormat ), format ),
        $"Unknown DDS Format Value: {formatValue:X}" );

      pict.Format = format;
    }

    private void ReadMipMaps( BinaryReader reader, S3DPicture pict )
    {
      pict.MipMapCount = reader.ReadInt32();
    }

    private void ReadData( BinaryReader reader, S3DPicture pict, long endOffset )
    {
      var dataSize = endOffset - reader.BaseStream.Position;

      pict.Data = new byte[ dataSize ];
      reader.Read( pict.Data, 0, ( int ) dataSize );
    }

    private enum PictureSentinel : ushort
    {
      Header = 0xF0,
      Dimensions = 0x0102,
      Format = 0xF2,
      MipMaps = 0xF9,
      Data = 0xFF,
      Footer = 0x01
    }

  }

}
