using System;
using H2AIndex.ViewModels.Abstract;

namespace H2AIndex.ViewModels
{

  public class DefaultViewModel : ViewModel, IDisposeWithView
  {

    #region Constructor

    public DefaultViewModel( IServiceProvider serviceProvider )
      : base( serviceProvider )
    {
    }

    #endregion

  }

}
