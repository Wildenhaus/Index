using System;

namespace H2AIndex.UI.Modals
{

  public partial class ExceptionModal : WindowModal
  {

    public ExceptionModal( Exception exception )
    {
      InitializeComponent();
      DataContext = exception;
    }

  }

}
