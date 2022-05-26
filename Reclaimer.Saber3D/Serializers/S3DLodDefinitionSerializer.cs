using System.Collections.Generic;
using System.IO;
using Saber3D.Data;

namespace Saber3D.Serializers
{

  public class S3DLodDefinitionSerializer : SerializerBase<List<S3DLodDefinition>>
  {

    protected override void OnDeserialize( BinaryReader reader, List<S3DLodDefinition> lodDefs )
    {
      // Read Counts
      var count = reader.ReadUInt32();
      var propertyCount = reader.ReadUInt32();

      // Create Objects
      for ( var i = 0; i < count; i++ )
        lodDefs.Add( new S3DLodDefinition() );

      _ = reader.ReadByte(); // Delimiter
      ReadObjectIdProperty( reader, lodDefs );
      _ = reader.ReadByte(); // Delimiter
      ReadIndexProperty( reader, lodDefs );
      _ = reader.ReadByte(); // Delimiter
      ReadIsLastLodProperty( reader, lodDefs );
    }

    private void ReadObjectIdProperty( BinaryReader reader, List<S3DLodDefinition> lodDefs )
    {
      for ( var i = 0; i < lodDefs.Count; i++ )
        lodDefs[ i ].ObjectId = reader.ReadUInt16();
    }

    private void ReadIndexProperty( BinaryReader reader, List<S3DLodDefinition> lodDefs )
    {
      for ( var i = 0; i < lodDefs.Count; i++ )
        lodDefs[ i ].Index = reader.ReadByte();
    }

    private void ReadIsLastLodProperty( BinaryReader reader, List<S3DLodDefinition> lodDefs )
    {
      for ( var i = 0; i < lodDefs.Count; i++ )
        lodDefs[ i ].IsLastLodUpToInfinity = reader.ReadBoolean();
    }

  }

}
