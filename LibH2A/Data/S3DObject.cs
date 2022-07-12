using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Saber3D.Common;

namespace Saber3D.Data
{

  public class S3DObject
  {

    #region Properties

    public S3DGeometryGraph GeometryGraph { get; }

    public short Id { get; set; }
    public string ReadName { get; set; }

    public short ParentId { get; set; }
    public short NextId { get; set; }
    public short PrevId { get; set; }
    public short ChildId { get; set; }

    public short AnimNumber { get; set; }
    public string ReadAffixes { get; set; }

    public Matrix4x4 MatrixLT { get; set; }
    public Matrix4x4 MatrixModel { get; set; }

    public S3DObjectGeometryUnshared GeomData { get; set; }

    public string UnkName { get; set; }
    // OBB
    public string Name { get; set; }
    public string Affixes { get; set; }

    public S3DObject Parent
    {
      get
      {
        if ( ParentId < 0 )
          return null;

        return GeometryGraph.Objects[ ParentId ];
      }
    }

    public IEnumerable<S3DGeometrySubMesh> SubMeshes
    {
      get => GeometryGraph.SubMeshes.Where( x => x.NodeId == Id );
    }

    #endregion

    #region Constructor

    public S3DObject( S3DGeometryGraph geometryGraph )
    {
      GeometryGraph = geometryGraph;
    }

    #endregion

    #region Public Methods

    public IEnumerable<S3DObject> EnumerateChildren()
    {
      var visited = new HashSet<short>();

      var currentId = ChildId;
      while ( visited.Add( currentId ) )
      {
        if ( currentId < 0 )
          break;

        yield return GeometryGraph.Objects[ currentId ];
        currentId = GeometryGraph.Objects[ currentId ].NextId;
      }
    }

    public string GetName()
    {
      if ( !string.IsNullOrWhiteSpace( UnkName ) )
        return UnkName.Split( new[] { "|" }, System.StringSplitOptions.RemoveEmptyEntries ).Last();

      if ( !string.IsNullOrWhiteSpace( Name ) )
        return Name;

      if ( !string.IsNullOrWhiteSpace( ReadName ) )
        return ReadName;

      return null;
    }

    public string GetParentName()
      => Parent.GetName();

    public string GetMeshName()
    {

      if ( string.IsNullOrWhiteSpace( UnkName ) )
        return null;

      var nameParts = UnkName.Split( new[] { "|" }, System.StringSplitOptions.RemoveEmptyEntries )
        .Where( x => x != "h" && !x.StartsWith( "_b_" ) && !x.StartsWith( "_m_" ) );

      return string.Join( "_", nameParts.TakeLast( 2 ) ).Trim();
    }

    public string GetParentMeshName()
    {

      if ( string.IsNullOrWhiteSpace( UnkName ) )
        return null;

      var nameParts = UnkName.Split( new[] { "|" }, System.StringSplitOptions.RemoveEmptyEntries )
        .Where( x => x != "h" && !x.StartsWith( "_b_" ) && !x.StartsWith( "_m_" ) );

      return nameParts.FirstOrDefault();
    }

    public string GetBoneName()
    {
      if ( string.IsNullOrWhiteSpace( UnkName ) )
        return null;

      var nameParts = UnkName.Split( new[] { "|" }, System.StringSplitOptions.RemoveEmptyEntries )
        .Where( x => x == "h" || x.StartsWith( "_b_" ) || x.StartsWith( "_m_" ) );

      return nameParts.LastOrDefault();
    }

    #endregion

  }

}
