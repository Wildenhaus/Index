using System.Diagnostics;
using System.Reflection;
using System.Windows.Documents;
using Index.UI.Controls;

namespace Index.Modals
{

  public partial class AboutModal : WindowedModal
  {

    public string VersionString
    {
      get
      {
        var version = Assembly.GetExecutingAssembly().GetName().Version;
        return $"v{version.Major}.{version.Minor}.{version.Revision}";
      }
    }

    #region Constructor

    public AboutModal( ContentHost host )
      : base( host )
    {
      InitializeComponent();
      DataContext = this;
    }

    #endregion

    #region Event Handlers

    private void OnHausClick( object sender, System.Windows.Input.MouseButtonEventArgs e )
      => Process.Start( new ProcessStartInfo( "https://ko-fi.com/hausdev" ) { UseShellExecute = true } );

    private void OnHyperlinkClick( object sender, System.Windows.RoutedEventArgs e )
    {
      var hyperlink = sender as Hyperlink;
      if ( hyperlink is null )
        return;

      var url = hyperlink.NavigateUri;
      Process.Start( new ProcessStartInfo( url.AbsoluteUri ) { UseShellExecute = true } );
    }

    #endregion


  }

}
