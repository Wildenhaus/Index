using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aspose.ThreeD;
using Aspose.ThreeD.Entities;
using Aspose.ThreeD.Utilities;
using Saber3D.Data;

namespace Testbed
{

  public class ArmatureExportTest
  {

    private S3DGeometryGraph _graph;

    private Scene _scene;

    public ArmatureExportTest( S3DGeometryGraph graph )
    {
      _graph = graph;
      _scene = new Scene();

      Init();
    }

    public void Save( Stream stream )
    {
      _scene.Save( stream, FileFormat.FBX7700Binary );
    }

    private void Init()
    {
      var rootObj = _graph.Objects[ _graph.RootNodeIndex ];

      var rootBone = new Skeleton( rootObj.Name );
      var armatureNode = new Node( rootObj.Name, rootBone );
      _scene.RootNode.AddChildNode( armatureNode );

      foreach ( var child in rootObj.EnumerateChildren( _graph ) )
        AddBone( armatureNode, child );
    }

    private void AddBone( Node parent, S3DObject obj )
    {
      if ( _graph.SubMeshes.Any( x => x.NodeId == obj.Id ) )
        return;

      var bone = new Skeleton( obj.Name ) { Type = SkeletonType.Bone };
      var boneNode = new Node( obj.Name, bone );
      parent.AddChildNode( boneNode );

      var matr = obj.MatrixLT;
      var matr2 = obj.MatrixModel;

      Console.WriteLine( obj.Name );
      Console.WriteLine( "  {0: 0.00000;-0.00000; 0.00000} {1: 0.00000;-0.00000; 0.00000} {2: 0.00000;-0.00000; 0.00000} {3: 0.00000;-0.00000; 0.00000}", matr.M11, matr.M12, matr.M13, matr.M14 );
      Console.WriteLine( "  {0: 0.00000;-0.00000; 0.00000} {1: 0.00000;-0.00000; 0.00000} {2: 0.00000;-0.00000; 0.00000} {3: 0.00000;-0.00000; 0.00000}", matr.M21, matr.M22, matr.M23, matr.M24 );
      Console.WriteLine( "  {0: 0.00000;-0.00000; 0.00000} {1: 0.00000;-0.00000; 0.00000} {2: 0.00000;-0.00000; 0.00000} {3: 0.00000;-0.00000; 0.00000}", matr.M31, matr.M32, matr.M33, matr.M34 );
      Console.WriteLine( "  {0: 0.00000;-0.00000; 0.00000} {1: 0.00000;-0.00000; 0.00000} {2: 0.00000;-0.00000; 0.00000} {3: 0.00000;-0.00000; 0.00000}", matr.M41, matr.M42, matr.M43, matr.M44 );
      Console.WriteLine();

      var ltMatr = new Matrix4(
        matr.M11, matr.M12, matr.M13, matr.M14,
        matr.M21, matr.M22, matr.M23, matr.M24,
        matr.M31, matr.M32, matr.M33, matr.M34,

        matr.M41, matr.M42, matr.M43, matr.M44
        );

      //boneNode.Transform.TransformMatrix = ltMatr;
      boneNode.Transform.TransformMatrix = ltMatr;

      foreach ( var child in obj.EnumerateChildren( _graph ) )
        AddBone( boneNode, child );
    }

  }

  public static class S3DObjectEx
  {

    public static IEnumerable<S3DObject> EnumerateChildren( this S3DObject obj, S3DGeometryGraph graph )
    {
      var visited = new HashSet<short>();

      var currentId = obj.ChildId;
      while ( visited.Add( currentId ) )
      {
        if ( currentId < 0 )
          break;

        yield return graph.Objects[ currentId ];
        currentId = graph.Objects[ currentId ].NextId;
      }
    }

  }

}
