using System;
using System.Collections.Generic;

namespace Saber3D.Files
{

  public interface IS3DFile : IDisposable, IEquatable<IS3DFile>
  {

    #region Properties

    IS3DFile Parent { get; }
    IEnumerable<IS3DFile> Children { get; }

    string Name { get; }
    string Extension { get; }
    long SizeInBytes { get; }

    string FileTypeDisplay { get; }

    #endregion

    #region Public Methods

    H2AStream GetStream();

    #endregion

  }

}
