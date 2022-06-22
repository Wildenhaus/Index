using System;
using System.Windows.Input;
using H2AIndex.Views;

namespace H2AIndex.Models
{

  public interface ITab : IDisposable
  {

    #region Events

    event EventHandler CloseRequested;

    #endregion

    #region Properties

    string Name { get; }
    IView View { get; }

    ICommand CloseCommand { get; }

    #endregion

    #region Public Methods

    void Close();

    #endregion

  }

}
