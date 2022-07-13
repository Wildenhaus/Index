using System.IO;
using System.Threading.Tasks;
using DirectXTexNet;
using H2AIndex.Models;
using ImageMagick;
using Saber3D.Data.Textures;
using Saber3D.Files;

namespace H2AIndex.Services
{

  public interface ITextureConversionService
  {

    S3DPicture DeserializeFile( IS3DFile file );

    Task<TextureModel> LoadTexture( IS3DFile file );
    Task<TextureModel> LoadTexture( string name, S3DPicture file );

    Task<Stream> GetDDSStream( IS3DFile file );
    Task<Stream> GetJpgStream( IS3DFile file, float quality = 1f );

    Task<MagickImageCollection> CreateMagickImageCollection( ScratchImage ddsImage );
    Task InvertGreenChannel( IMagickImage<float> image );
    Task RecalculateZChannel( IMagickImage<float> image );

  }

}
