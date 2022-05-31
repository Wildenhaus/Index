using System.Collections.Generic;
using System.IO;
using Saber3D.Common;
using Saber3D.Data;

namespace Saber3D.Serializers
{

  public class S3DObjectLodRootSerializer : SerializerBase<List<S3DObjectLodRoot>>
  {

    private S3DGeometryGraph GeometryGraph { get; }

    protected override void OnDeserialize( BinaryReader reader, List<S3DObjectLodRoot> lodRoots )
    {
      var count = reader.ReadUInt32();
      var propertyCount = reader.ReadUInt32();

      for ( var i = 0; i < count; i++ )
        lodRoots.Add( new S3DObjectLodRoot() );

      ReadObjectIdsProperty( reader, lodRoots );
      ReadMaxObjectLodIndicesProperty( reader, lodRoots );
      ReadLodDistancesProperty( reader, lodRoots );
      ReadBoundingBoxProperty( reader, lodRoots );
      // TODO: skip[float] property?
    }

    private void ReadObjectIdsProperty( BinaryReader reader, List<S3DObjectLodRoot> lodRoots )
    {
      if ( reader.ReadByte() == 0 )
        return;

      foreach ( var lodRoot in lodRoots )
      {
        var count = reader.ReadInt32();
        lodRoot.ObjectIds = new List<uint>( count );
        for ( var i = 0; i < count; i++ )
          lodRoot.ObjectIds.Add( reader.ReadUInt32() );
      }
    }

    private void ReadMaxObjectLodIndicesProperty( BinaryReader reader, List<S3DObjectLodRoot> lodRoots )
    {
      if ( reader.ReadByte() == 0 )
        return;

      foreach ( var lodRoot in lodRoots )
      {
        var count = reader.ReadInt32();
        lodRoot.MaxObjectLodIndices = new List<uint>( count );
        for ( var i = 0; i < count; i++ )
          lodRoot.MaxObjectLodIndices.Add( reader.ReadUInt32() );
      }
    }

    private void ReadLodDistancesProperty( BinaryReader reader, List<S3DObjectLodRoot> lodRoots )
    {
      if ( reader.ReadByte() == 0 )
        return;

      var serializer = new S3DLodDistanceSerializer();
      foreach ( var lodRoot in lodRoots )
        lodRoot.LodDistances = serializer.Deserialize( reader );
    }

    private void ReadBoundingBoxProperty( BinaryReader reader, List<S3DObjectLodRoot> lodRoots )
    {
      if ( reader.ReadByte() == 0 )
        return;

      foreach ( var lodRoot in lodRoots )
        lodRoot.BoundingBox = new M3DBox( reader.ReadVector3(), reader.ReadVector3() );
    }

  }

}
