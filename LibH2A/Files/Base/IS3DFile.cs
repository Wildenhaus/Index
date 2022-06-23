﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Saber3D.Files
{

  public interface IS3DFile : IDisposable, IEquatable<IS3DFile>
  {

    #region Properties

    string Name { get; }
    string Extension { get; }

    IS3DFile Parent { get; }
    IEnumerable<IS3DFile> Children { get; }

    string FileTypeDisplay { get; }

    H2AFileContext FileContext { get; }

    #endregion

    #region Public Methods

    Stream GetStream();
    void SetFileContext( H2AFileContext fileContext );

    #endregion

  }

}
