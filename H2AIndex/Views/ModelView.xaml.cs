using System.Windows;
using H2AIndex.ViewModels;
using HelixToolkit.Wpf.SharpDX;

namespace H2AIndex.Views
{

  public partial class ModelView : View<ModelViewModel>
  {

    public ModelView()
    {
      InitializeComponent();

      Viewport.AddHandler( Element3D.MouseDown3DEvent, new RoutedEventHandler( ( s, e ) =>
      {
        var arg = e as MouseDown3DEventArgs;

        if ( arg.HitTestResult == null )
          return;
      } ) );
    }

  }

}
