using System;
using System.Reflection;
using System.Windows.Input;
using H2AIndex.Common;
using H2AIndex.ViewModels.Abstract;

namespace H2AIndex.ViewModels
{

  public class AboutViewModel : ViewModel, IDisposeWithView
  {

    #region Properties

    public string VersionString
    {
      get
      {
        var version = ((App)App.Current).Version;
        return $"v{version}";
      }
    }

    #endregion

    #region Constructor

    public AboutViewModel( IServiceProvider serviceProvider )
      : base( serviceProvider )
    {
    }

    #endregion

  }

}
