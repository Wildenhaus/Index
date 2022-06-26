using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using DirectXTexNet;
using Saber3D.Data.Textures;

namespace Index.Tools
{

  public static class TextureConverter
  {

    public static byte[] ConvertToDDS( S3DPicture pict )
    {
      using ( var img = CreateScratchImage( pict ) )
      using ( var imgStream = img.SaveToDDSMemory( DDS_FLAGS.NONE ) )
      using ( var outStream = new MemoryStream() )
      {
        imgStream.CopyTo( outStream );
        return outStream.ToArray();
      }
    }

    public static byte[] ConvertToJpg( S3DPicture pict, int imageIndex, float quality = .5f )
    {
      var image = CreateScratchImage( pict );

      _ = DecompressIfNeeded( ref image );
      _ = ConvertIfNeeded( ref image );

      using ( image )
      using ( var imgStream = image.SaveToJPGMemory( imageIndex, quality ) )
      using ( var outStream = new MemoryStream() )
      {
        imgStream.CopyTo( outStream );
        return outStream.ToArray();
      }
    }

    public static byte[] ConvertToTGA( S3DPicture pict, int imageIndex )
    {
      var image = CreateScratchImage( pict );

      _ = DecompressIfNeeded( ref image );
      _ = ConvertIfNeeded( ref image );

      using ( image )
      using ( var imgStream = image.SaveToTGAMemory( imageIndex ) )
      using ( var outStream = new MemoryStream() )
      {
        imgStream.CopyTo( outStream );
        return outStream.ToArray();
      }
    }

    public static BitmapImage ConvertToBitmap( S3DPicture pict, float quality = 1f )
    {
      var image = CreateScratchImage( pict );

      _ = DecompressIfNeeded( ref image );
      _ = ConvertIfNeeded( ref image );

      using ( image )
      {
        using ( var imgStream = image.SaveToJPGMemory( 0, quality ) )
        using ( var memStream = new MemoryStream() )
        {
          imgStream.CopyTo( memStream );
          memStream.Position = 0;

          var bitmap = new BitmapImage();
          bitmap.BeginInit();
          bitmap.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
          bitmap.CacheOption = BitmapCacheOption.OnLoad;
          bitmap.StreamSource = memStream;
          bitmap.UriSource = null;
          bitmap.EndInit();

          bitmap.Freeze();
          return bitmap;
        }
      }
    }

    public static IEnumerable<BitmapImage> ConvertToBitmaps( S3DPicture pict, float quality = 1f )
    {
      var image = CreateScratchImage( pict );

      _ = DecompressIfNeeded( ref image );
      _ = ConvertIfNeeded( ref image );

      using ( image )
      {
        var imageCount = image.GetImageCount();

        for ( var i = 0; i < imageCount; i++ )
        {
          using ( var imgStream = image.SaveToJPGMemory( i, quality ) )
          using ( var memStream = new MemoryStream() )
          {
            imgStream.CopyTo( memStream );
            memStream.Position = 0;

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = memStream;
            bitmap.UriSource = null;
            bitmap.EndInit();

            bitmap.Freeze();
            yield return bitmap;
          }
        }
      }

      image?.Dispose();
    }

    private static ScratchImage CreateScratchImage( S3DPicture pict )
    {
      var format = GetDxgiFormat( pict.Format );

      // Create a scratch image based on the face count
      ScratchImage img;
      if ( pict.Faces == 1 )
        img = TexHelper.Instance.Initialize2D( format, pict.Width, pict.Height, 1, pict.MipMapCount, 0 );
      else
        img = TexHelper.Instance.InitializeCube( format, pict.Width, pict.Height, pict.Faces, pict.MipMapCount, 0 );

      // Get a pointer to the scratch image's raw pixel data
      var srcData = pict.Data;
      var pDestLen = img.GetPixelsSize();
      var pDest = img.GetPixels();
      Debug.Assert( pDestLen >= srcData.Length, "Source data will not fit in the destination image." );

      // Copy the data into the image
      Marshal.Copy( srcData, 0, pDest, srcData.Length );

      return img;
    }

    private static bool DecompressIfNeeded( ref ScratchImage image, DXGI_FORMAT outFormat = DXGI_FORMAT.R8G8B8A8_UNORM )
    {
      var format = image.GetMetadata().Format;

      if ( !format.ToString().StartsWith( "BC" ) )
        return false;

      var decompressedImage = image.Decompress( outFormat );
      image.Dispose();
      image = decompressedImage;

      return true;
    }

    private static bool ConvertIfNeeded( ref ScratchImage image, DXGI_FORMAT outFormat = DXGI_FORMAT.R8G8B8A8_UNORM )
    {
      var format = image.GetMetadata().Format;

      // These formats aren't supported by TGA. We need to convert.
      switch ( format )
      {
        case DXGI_FORMAT.R16G16B16A16_FLOAT:
        case DXGI_FORMAT.R8G8_UNORM:
        case DXGI_FORMAT.R9G9B9E5_SHAREDEXP:
          var convertedImg = image.Convert( outFormat, TEX_FILTER_FLAGS.DEFAULT, 0 );
          image.Dispose();
          image = convertedImg;
          return true;
        default:
          return false;
      }
    }

    private static DXGI_FORMAT GetDxgiFormat( S3DPictureFormat format )
    {
      switch ( format )
      {
        case S3DPictureFormat.A8R8G8B8:
          return DXGI_FORMAT.R8G8B8A8_UNORM;
        case S3DPictureFormat.A8L8:
          return DXGI_FORMAT.R8G8_UNORM;
        case S3DPictureFormat.OXT1:
        case S3DPictureFormat.AXT1:
          return DXGI_FORMAT.BC1_UNORM;
        case S3DPictureFormat.DXT3:
          return DXGI_FORMAT.BC2_UNORM;
        case S3DPictureFormat.DXT5:
          return DXGI_FORMAT.BC3_UNORM;
        case S3DPictureFormat.X8R8G8B8:
          return DXGI_FORMAT.B8G8R8X8_UNORM;
        case S3DPictureFormat.DXN:
          return DXGI_FORMAT.BC5_UNORM;
        case S3DPictureFormat.DXT5A:
          return DXGI_FORMAT.BC4_UNORM;
        case S3DPictureFormat.A16B16G16R16_F:
          return DXGI_FORMAT.R16G16B16A16_FLOAT;
        case S3DPictureFormat.R9G9B9E5_SHAREDEXP:
          return DXGI_FORMAT.R9G9B9E5_SHAREDEXP;
        default:
          throw new NotImplementedException();
      }
    }

  }

}
