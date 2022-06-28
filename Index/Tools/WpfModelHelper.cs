using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using Assimp;
using Index.ViewModels;
using Saber3D.Files;
using Saber3D.Serializers;

namespace Index.Tools
{

  public class WpfModelHelper
  {

    static Brush UvBrush;

    private Scene _assimpScene;

    private Model3DGroup _group;
    private DiffuseMaterial[] _diffuseMaterialCache;

    public ProgressViewModel Progress { get; private set; }

    public static Model3DGroup PrepareModelForViewer( Scene assimpScene, ProgressViewModel progress )
    {
      var helper = new WpfModelHelper( assimpScene, progress );

      return helper.Build();
    }

    static WpfModelHelper()
    {
      var b = new ImageBrush( new BitmapImage( new System.Uri( @"C:\Users\Reid\Desktop\uv.png" ) ) );
      UvBrush = b;
      b.Freeze();
    }

    private WpfModelHelper( Scene assimpScene, ProgressViewModel progress )
    {
      _assimpScene = assimpScene;
      _group = new Model3DGroup();

      Progress = progress;
    }

    private Model3DGroup Build()
    {
      var group = _group;
      var assimpMeshes = _assimpScene.Meshes;
      _diffuseMaterialCache = new DiffuseMaterial[ _assimpScene.MaterialCount ];

      Progress.UnitName = "meshes prepared";
      Progress.CompletedUnits = 0;
      Progress.TotalUnits = assimpMeshes.Count;
      Progress.IsIndeterminate = false;

      foreach ( var assimpMesh in _assimpScene.Meshes )
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
        foreach ( var uv in assimpMesh.TextureCoordinateChannels[ 0 ] )
          wpfMesh.TextureCoordinates.Add( new System.Windows.Point( uv.X, 1 - uv.Y ) );

        var geomModel = new GeometryModel3D();
        geomModel.Geometry = wpfMesh;
        geomModel.Material = new DiffuseMaterial( UvBrush );
        if ( TryGetDiffuseMaterial( assimpMesh.MaterialIndex, out var material ) )
          geomModel.Material = material;

        var t = new RotateTransform3D();
        t.Rotation = new AxisAngleRotation3D( new System.Windows.Media.Media3D.Vector3D( 1, 0, 0 ), 90 );

        geomModel.Transform = t;

        geomModel.Freeze();
        group.Children.Add( geomModel );
        Progress.CompletedUnits++;
      }

      _group.Freeze();
      return _group;
    }

    private bool TryGetDiffuseMaterial( int index, out DiffuseMaterial material )
    {
      material = default;

      if ( _diffuseMaterialCache[ index ] != null )
      {
        material = _diffuseMaterialCache[ index ];
        return true;
      }

      var assimpMat = _assimpScene.Materials[ index ];
      var matFileName = assimpMat.Name;
      var matFile = H2AFileContext.Global.GetFiles( $"{matFileName}.pct" )
        .Where( x => x.Name == $"{matFileName}.pct" )
        .FirstOrDefault();

      if ( matFile is null )
        return false;

      var stream = matFile.GetStream();
      var reader = new BinaryReader( stream );
      var pict = new S3DPictureSerializer().Deserialize( reader );

      var textureQuality = PreferencesManager.Preferences.TextureModelViewerQuality;
      var bitmapTexture = TextureConverter.ConvertToBitmap( pict, textureQuality );
      var brush = new ImageBrush( bitmapTexture )
      {
        ViewportUnits = BrushMappingMode.Absolute,
        TileMode = TileMode.Tile
      };
      material = new DiffuseMaterial( brush );

      brush.Freeze();
      material.Freeze();
      _diffuseMaterialCache[ index ] = material;
      return true;
    }

  }

}
