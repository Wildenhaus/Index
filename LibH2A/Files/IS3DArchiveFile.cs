using System.Collections.Generic;
using System.IO;

namespace Saber3D.Files
{

  public interface IS3DArchiveFile : IS3DFile
  {

    #region Properties

    IReadOnlyDictionary<string, IS3DArchiveFileEntry> Entries { get; }

    #endregion

    #region Public Methods

    Stream GetEntryStream( IS3DArchiveFileEntry entryFile );

    #endregion

  }

}
