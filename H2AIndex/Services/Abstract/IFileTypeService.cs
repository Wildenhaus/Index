using System;
using System.Collections.Generic;

namespace H2AIndex.Services
{

  public interface IFileTypeService
  {

    #region Data Members

    IReadOnlySet<string> ExtensionsWithEditorSupport { get; }

    #endregion

    #region Public Methods

    Type GetViewModelType( Type fileType );

    #endregion

  }

}
