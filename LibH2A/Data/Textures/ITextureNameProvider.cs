using System.Collections.Generic;

namespace Saber3D.Data.Textures
{

  public interface ITextureNameProvider
  {

    IEnumerable<string> GetTextureNames();

  }

}
