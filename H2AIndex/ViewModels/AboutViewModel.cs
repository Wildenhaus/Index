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
        var version = Assembly.GetExecutingAssembly().GetName().Version;
        return $"v{version.Major}.{version.Minor}.{version.Revision}";
      }
    }

    public ICommand OpenLinkCommand { get; set; }

    #endregion

    #region Constructor

    public AboutViewModel( IServiceProvider serviceProvider )
      : base( serviceProvider )
    {
      OpenLinkCommand = new Command<string>( OpenWebPage );
    }

    #endregion

  }

}
