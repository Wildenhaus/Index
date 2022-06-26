using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Index.UI.Controls;

namespace Index.Modals
{


  public partial class MessageModal : HostedModal
  {

    #region Data Members

    private TaskCompletionSource<string> _tcs;

    #endregion

    #region Properties

    public Task<string> AwaiterTask => _tcs.Task;

    #endregion

    public MessageModal( ContentHost host, string title, string message, IEnumerable<string> buttons = null )
      : base( host )
    {
      InitializeComponent();

      TitleLabel.Content = title;
      MessageLabel.Text = message;

      CreateButtons( buttons );

      _tcs = new TaskCompletionSource<string>();
    }

    private void CreateButtons( IEnumerable<string> buttons )
    {
      if ( buttons is null || !buttons.Any() )
        buttons = new string[] { "OK" };

      foreach ( var buttonText in buttons )
      {
        var button = new Button
        {
          Padding = new Thickness( 10, 5, 10, 5 ),
          Margin = new Thickness( 5 ),
          Content = buttonText
        };

        button.Click += OnButtonClick;

        ButtonPanel.Children.Add( button );
      }
    }

    private void OnButtonClick( object sender, RoutedEventArgs e )
    {
      var buttonText = ( sender as Button ).Content;
      _tcs.SetResult( ( string ) buttonText );

      Dispose();
    }

  }

}