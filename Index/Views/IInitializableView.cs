using System.Threading.Tasks;
using Index.Controls;

namespace Index.Views
{

  public interface IInitializableView
  {

    ContentHost Host { get; }

    Task Initialize( ContentHost host );

  }

}
