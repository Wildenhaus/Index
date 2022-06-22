using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Assimp;

namespace Index.Controls
{

  public partial class ModelViewerControl : UserControl
  {

    private Model3D _model;

    public ModelViewerControl( Scene assimpScene )
    {
      InitializeComponent();
      _model = ConvertAssimpScene( assimpScene );
      ModelContainer.Children.Add( new ModelVisual3D() { Content = _model } );
    }

    private static Model3D ConvertAssimpScene( Scene assimpScene )
    {
      var group = new Model3DGroup();

      foreach ( var assimpMesh in assimpScene.Meshes )
      {
        var wpfMesh = new MeshGeometry3D();
        foreach ( var vertex in assimpMesh.Vertices )
          wpfMesh.Positions.Add( new Point3D( vertex.X, vertex.Y, vertex.Z ) );
        foreach ( var face in assimpMesh.Faces )
        {
          wpfMesh.TriangleIndices.Add( face.Indices[ 0 ] );
          wpfMesh.TriangleIndices.Add( face.Indices[ 1 ] );
          wpfMesh.TriangleIndices.Add( face.Indices[ 2 ] );
        }

        var geomModel = new GeometryModel3D();
        geomModel.Geometry = wpfMesh;
        geomModel.Material = new DiffuseMaterial( Brushes.Gray );

        group.Children.Add( geomModel );
      }

      return group;
    }

  }
}
