using System.IO;
using System.Threading.Tasks;
using H2AIndex.Models;
using Saber3D.Files;

namespace H2AIndex.Services.Abstract
{

  public interface ITextureConversionService
  {

    Task<TextureModel> LoadTexture( IS3DFile file );

    Task<Stream> GetDDSStream( IS3DFile file );

  }

}
