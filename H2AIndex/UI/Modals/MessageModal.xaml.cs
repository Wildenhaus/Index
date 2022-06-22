namespace H2AIndex.UI.Modals
{

  public partial class MessageModal : WindowModal
  {

    public string Message { get; set; }

    public MessageModal()
    {
      InitializeComponent();
      DataContext = this;
    }

  }

}