using System;
using System.IO;

namespace Saber3D.Files
{

  public interface IS3DFile : IDisposable
  {

    #region Properties

    string Name { get; }

    H2AFileContext FileContext { get; }

    #endregion

    #region Public Methods

    Stream GetStream();

    #endregion

  }

}
