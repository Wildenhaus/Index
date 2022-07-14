using System.IO;
using System.Threading.Tasks;
using DirectXTexNet;
using H2AIndex.Models;
using ImageMagick;
using Saber3D.Data.Textures;
using Saber3D.Files.FileTypes;

namespace H2AIndex.Services
{

  public interface ITextureConversionService
  {

    Task<TextureModel> LoadTexture( PictureFile file );
    Task<TextureModel> LoadTexture( string name, S3DPicture file );

    Task<Stream> GetDDSStream( PictureFile file );
    Task<Stream> GetJpgStream( PictureFile file, float quality = 1f );

    Task<MagickImageCollection> CreateMagickImageCollection( ScratchImage ddsImage );
    Task InvertGreenChannel( IMagickImage<float> image );
    Task RecalculateZChannel( IMagickImage<float> image );

  }

}
