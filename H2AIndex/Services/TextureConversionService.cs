using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using DirectXTexNet;
using H2AIndex.Models;
using H2AIndex.Services.Abstract;
using Saber3D.Data.Textures;
using Saber3D.Files;
using Saber3D.Serializers;

namespace H2AIndex.Services
{

  public class TextureConversionService : ITextureConversionService
  {

    public async Task<TextureModel> LoadTexture( IS3DFile file )
    {
      var pict = DeserializeFile( file );

      var ddsImage = CreateDDS( pict );
      var metadata = ddsImage.GetMetadata();

      var name = Path.GetFileNameWithoutExtension( file.Name );
      var model = TextureModel.Create( name, ddsImage, metadata );

      await CreateImagePreviews( ddsImage, model );

      return model;
    }

    public async Task<Stream> GetDDSStream( IS3DFile file )
    {
      var pict = DeserializeFile( file );

      var outStream = new MemoryStream();
      using ( var ddsImage = CreateDDS( pict ) )
      using ( var ddsStream = ddsImage.SaveToDDSMemory( DDS_FLAGS.NONE ) )
      {
        ddsStream.CopyTo( outStream );
        outStream.Position = 0;
        return outStream;
      }
    }


    #region Private Methods

    private S3DPicture DeserializeFile( IS3DFile file )
    {
      var stream = file.GetStream();
      var reader = new BinaryReader( stream );

      return new S3DPictureSerializer().Deserialize( reader );
    }

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
      using var compatImage = PrepareNonDDSImage( ddsImage );

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
    }

    private ScratchImage PrepareNonDDSImage( ScratchImage ddsImage )
    {
      var format = ddsImage.GetMetadata().Format;

      if ( format.ToString().StartsWith( "BC" ) )
        return ddsImage.Decompress( DXGI_FORMAT.R16G16B16A16_UNORM );
      else
        return ddsImage.Convert( DXGI_FORMAT.R16G16B16A16_UNORM, TEX_FILTER_FLAGS.DEFAULT, 0 );
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
