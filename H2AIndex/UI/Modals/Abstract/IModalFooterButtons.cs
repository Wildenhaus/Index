using System.Collections.Generic;
using System.Windows.Controls;

namespace H2AIndex.UI.Modals
{

  public interface IModalFooterButtons
  {

    IEnumerable<Button> GetFooterButtons();

  }

}
