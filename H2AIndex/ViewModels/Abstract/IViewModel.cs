using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using H2AIndex.UI.Modals;

namespace H2AIndex.ViewModels
{

  public interface IViewModel : IDisposable
  {

    ObservableCollection<IModal> Modals { get; }

    Task Initialize();

  }

}
