using System.Collections.Generic;
using Index.UI.Controls;

namespace Index.Modals
{


  public partial class MessageModal : WindowedModal
  {

    public MessageModal( ContentHost host, string title, string message, IEnumerable<string> buttons = null )
      : base( host, buttons )
    {
      InitializeComponent();

      Title = title;
      MessageLabel.Text = message;
    }

  }

}