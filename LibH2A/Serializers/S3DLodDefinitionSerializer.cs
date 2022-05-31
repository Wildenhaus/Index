using System.Collections.Generic;
using System.IO;
using Saber3D.Data;
using static Saber3D.Assertions;

namespace Saber3D.Serializers
{

  public class S3DLodDefinitionSerializer : SerializerBase<List<S3DLodDefinition>>
  {

    protected override void OnDeserialize( BinaryReader reader, List<S3DLodDefinition> lodDefs )
    {
      // Read Counts
      var count = reader.ReadUInt32();
      var propertyCount = reader.ReadUInt32();
      Assert( count < 0x1000 );

      // Create Objects
      for ( var i = 0; i < count; i++ )
        lodDefs.Add( new S3DLodDefinition() );

      _ = reader.ReadByte(); // TODO: Delimiter, verify
      ReadObjectIdProperty( reader, lodDefs );
      _ = reader.ReadByte(); // TODO: Delimiter, verify
      ReadIndexProperty( reader, lodDefs );
      _ = reader.ReadByte(); // TODO: Delimiter, verify
      ReadIsLastLodProperty( reader, lodDefs );
    }

    private void ReadObjectIdProperty( BinaryReader reader, List<S3DLodDefinition> lodDefs )
    {
      foreach ( var lodDef in lodDefs )
        lodDef.ObjectId = reader.ReadUInt16();
    }

    private void ReadIndexProperty( BinaryReader reader, List<S3DLodDefinition> lodDefs )
    {
      foreach ( var lodDef in lodDefs )
        lodDef.Index = reader.ReadByte();
    }

    private void ReadIsLastLodProperty( BinaryReader reader, List<S3DLodDefinition> lodDefs )
    {
      foreach ( var lodDef in lodDefs )
        lodDef.IsLastLodUpToInfinity = reader.ReadBoolean();
    }

  }

}
