using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using DirectXTexNet;
using H2AIndex.Models;
using ImageMagick;
using Saber3D.Data.Textures;
using Saber3D.Files.FileTypes;

namespace H2AIndex.Services
{

  public class TextureConversionService : ITextureConversionService
  {

    public Task<TextureModel> LoadTexture( PictureFile file )
    {
      var pict = file.Deserialize();
      return LoadTexture( file.Name, pict );
    }

    public async Task<TextureModel> LoadTexture( string fileName, S3DPicture file )
    {
      var ddsImage = CreateDDS( file );
      var metadata = ddsImage.GetMetadata();

      var name = Path.GetFileNameWithoutExtension( fileName );
      var model = TextureModel.Create( name, file, ddsImage, metadata );

      await CreateImagePreviews( ddsImage, model );

      return model;
    }

    public async Task<Stream> GetDDSStream( PictureFile file )
    {
      var pict = file.Deserialize();

      var outStream = new MemoryStream();
      using ( var ddsImage = CreateDDS( pict ) )
      using ( var ddsStream = ddsImage.SaveToDDSMemory( DDS_FLAGS.NONE ) )
      {
        ddsStream.CopyTo( outStream );
        outStream.Position = 0;
        return outStream;
      }
    }

    public async Task<Stream> GetJpgStream( PictureFile file, float quality = 1f )
    {
      var pict = file.Deserialize();

      var outStream = new MemoryStream();
      using ( var ddsImage = CreateDDS( pict ) )
      using ( var compatImage = PrepareNonDDSImage( ddsImage ) )
      using ( var jpgStream = compatImage.SaveToJPGMemory( 0, quality ) )
      {
        jpgStream.CopyTo( outStream );
        outStream.Position = 0;
        return outStream;
      }
    }

    public async Task<MagickImageCollection> CreateMagickImageCollection( ScratchImage ddsImage, IEnumerable<int> indices = null )
    {
      var convertWasRequired = PrepareNonDDSImage( ddsImage, out var decompressedImage );

      var imageCount = ddsImage.GetImageCount();
      if ( indices is null )
        indices = Enumerable.Range( 0, imageCount );

      var idxSet = indices.ToHashSet();
      var imageDict = new ConcurrentDictionary<int, MagickImage>();
      Parallel.For( 0, imageCount, i =>
      {
        if ( !idxSet.Contains( i ) )
        {
          imageDict[ i ] = new MagickImage();
          return;
        }

        using ( var imageStream = decompressedImage.SaveToDDSMemory( i, DDS_FLAGS.NONE ) )
        {
          var image = new MagickImage( imageStream );
          imageDict[ i ] = image;
        }
      } );

      if ( convertWasRequired )
        decompressedImage?.Dispose();

      var collection = new MagickImageCollection();
      for ( var i = 0; i < imageCount; i++ )
        collection.Add( imageDict[ i ] );

      return collection;
    }

    public async Task InvertGreenChannel( IMagickImage<float> image )
    {
      var channelCount = image.ChannelCount;
      var height = image.Height;
      var width = image.Width;

      using var pixels = image.GetPixels();
      for ( var h = 0; h < height; h++ )
      {
        var row = pixels.GetArea( 0, h, width, 1 );
        for ( var w = 0; w < row.Length; w += channelCount )
        {
          var y = row[ w + 1 ] / 65535.0f;
          y = 1.0f - y;
          row[ w + 1 ] = y * 65535.0f;
        }
        pixels.SetArea( 0, h, width, 1, row );
      }
    }

    public async Task RecalculateZChannel( IMagickImage<float> image )
    {
      //image.ColorSpace = ColorSpace.sRGB;
      var channelCount = image.ChannelCount;
      var height = image.Height;
      var width = image.Width;

      using var pixels = image.GetPixels();
      for ( var h = 0; h < height; h++ )
      {
        var row = pixels.GetArea( 0, h, width, 1 );
        for ( var w = 0; w < row.Length; w += channelCount )
        {
          var x = row[ w + 0 ] / 65535.0f;
          var y = row[ w + 1 ] / 65535.0f;
          var z = MathF.Sqrt( 1 - ( x * x ) + ( y * y ) );
          z = MathF.Max( 0, MathF.Min( 1, z ) ) * 65535.0f;

          row[ w + 2 ] = z;
        }
        pixels.SetArea( 0, h, width, 1, row );
      }
    }

    #region Private Methods

    private ScratchImage CreateDDS( S3DPicture pict )
    {
      var format = GetDxgiFormat( pict.Format );

      // Create a scratch image based on the face count
      ScratchImage img;
      if ( pict.Faces == 1 )
        img = TexHelper.Instance.Initialize2D( format, pict.Width, pict.Height, 1, pict.MipMapCount, CP_FLAGS.NONE );
      else
        img = TexHelper.Instance.InitializeCube( format, pict.Width, pict.Height, pict.Faces, pict.MipMapCount, CP_FLAGS.NONE );

      // Get a pointer to the scratch image's raw pixel data
      var srcData = pict.Data;
      var pDestLen = img.GetPixelsSize();
      var pDest = img.GetPixels();
      Debug.Assert( pDestLen >= srcData.Length, "Source data will not fit in the destination image." );

      // Copy the data into the image
      Marshal.Copy( srcData, 0, pDest, srcData.Length );

      return img;
    }

    private async Task CreateImagePreviews( ScratchImage ddsImage, TextureModel model )
    {
      var convertWasRequired = PrepareNonDDSImage( ddsImage, out var compatImage );

      foreach ( var image in model.Faces )
      {
        foreach ( var mip in image.MipMaps )
        {
          var mem = new MemoryStream();
          using ( var jpgStream = compatImage.SaveToJPGMemory( mip.ImageIndex, 1f ) )
          {
            await jpgStream.CopyToAsync( mem );
            mem.Position = 0;

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
            bitmap.CacheOption = BitmapCacheOption.Default;
            bitmap.StreamSource = mem;
            bitmap.UriSource = null;
            bitmap.EndInit();

            bitmap.Freeze();
            mip.Preview = bitmap;
          }
        }
      }

      if ( convertWasRequired )
        compatImage?.Dispose();
    }

    private bool PrepareNonDDSImage( ScratchImage ddsImage, out ScratchImage rgbImage )
    {
      rgbImage = ddsImage;
      var format = ddsImage.GetMetadata().Format;
      if ( format == DXGI_FORMAT.R8G8B8A8_UNORM )
        return false;

      if ( format.ToString().StartsWith( "BC" ) )
        rgbImage = ddsImage.Decompress( DXGI_FORMAT.R8G8B8A8_UNORM );
      else
        rgbImage = ddsImage.Convert( DXGI_FORMAT.R8G8B8A8_UNORM, TEX_FILTER_FLAGS.DEFAULT, 0 );

      return true;
    }

    [Obsolete( "You should only use this if you are planning on tossing the result." )]
    private ScratchImage PrepareNonDDSImage( ScratchImage ddsImage )
    {
      var format = ddsImage.GetMetadata().Format;
      if ( format == DXGI_FORMAT.R8G8B8A8_UNORM )
        return ddsImage;

      if ( format.ToString().StartsWith( "BC" ) )
        return ddsImage.Decompress( DXGI_FORMAT.R8G8B8A8_UNORM );
      else
        return ddsImage.Convert( DXGI_FORMAT.R8G8B8A8_UNORM, TEX_FILTER_FLAGS.DEFAULT, 0 );
    }

    private DXGI_FORMAT GetDxgiFormat( S3DPictureFormat format )
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

    #endregion

  }

}
