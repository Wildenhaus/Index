using H2AIndex.Models;
using Saber3D.Files;

namespace H2AIndex.Services.Abstract
{

  public interface ITabService
  {

    #region Properties

    TabContextModel TabContext { get; }

    #endregion

    #region Public Methods

    bool CreateTabForFile( IS3DFile file, out ITab tab );

    #endregion

  }

}
