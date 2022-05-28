using System.Collections.Generic;
using System.IO;
using Saber3D.Data;
using static Saber3D.Assertions;

namespace Saber3D.Serializers
{

  public class S3DObjectLodRootSerializer : SerializerBase<List<S3DObjectLodRoot>>
  {

    private S3DGeometryGraph GeometryGraph { get; }

    public S3DObjectLodRootSerializer( S3DGeometryGraph geometryGraph )
    {
      GeometryGraph = geometryGraph;
    }

    protected override void OnDeserialize( BinaryReader reader, List<S3DObjectLodRoot> lodRoots )
    {
      var count = GeometryGraph.Objects.Count;

      for ( var i = 0; i < count; i++ )
        lodRoots.Add( new S3DObjectLodRoot() );

      ReadObjectIdProperty( reader, lodRoots );
    }

    private void ReadObjectIdProperty( BinaryReader reader, List<S3DObjectLodRoot> lodRoots )
    {
      var endOffset = reader.ReadUInt32();

      for ( var i = 0; i < lodRoots.Count; i++ )
        lodRoots[ i ].ObjectId = reader.ReadInt16();

      Assert( reader.BaseStream.Position == endOffset );
    }

  }

}
