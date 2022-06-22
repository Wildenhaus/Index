using System;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace H2AIndex.UI.Modals
{

  public interface IModal : IAddChild, IDisposable
  {

    #region Properties

    Task<object> Task { get; }

    #endregion

    #region Public Methods

    Task Show();
    Task Hide();

    #endregion

  }

}
