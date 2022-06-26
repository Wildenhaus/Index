using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;
using PropertyChanged;

namespace Index.ViewModels
{

  [AddINotifyPropertyChangedInterface]
  public class ModelViewViewModel : ViewModel
  {

    public string Name { get; set; }

    public Model3DGroup Model
    {
      get;
      set;
    }

    [DependsOn( nameof( Model ) )]
    public int MeshCount
    {
      get => Meshes.Count();
    }

    [DependsOn( nameof( Model ) )]
    public int VertexCount
    {
      get => Meshes.Sum( x => x.Positions.Count );
    }

    [DependsOn( nameof( Model ) )]
    public int FaceCount
    {
      get => Meshes.Sum( x => x.TriangleIndices.Count / 3 );
    }

    private IEnumerable<MeshGeometry3D> Meshes
    {
      get
      {
        if ( Model is null )
          return Enumerable.Empty<MeshGeometry3D>();

        var geometry = Model.Children.OfType<GeometryModel3D>().Select( x => x.Geometry );
        return geometry.OfType<MeshGeometry3D>();
      }
    }

    protected override void OnDisposing()
    {
      Model = null;
    }

  }

}
