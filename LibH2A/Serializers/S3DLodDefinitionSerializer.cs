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

      ReadObjectIdProperty( reader, lodDefs );
      ReadIndexProperty( reader, lodDefs );
      ReadIsLastLodProperty( reader, lodDefs );
    }

    private void ReadObjectIdProperty( BinaryReader reader, List<S3DLodDefinition> lodDefs )
    {
      if ( reader.ReadByte() == 0 )
        return;

      foreach ( var lodDef in lodDefs )
        lodDef.ObjectId = reader.ReadUInt16();
    }

    private void ReadIndexProperty( BinaryReader reader, List<S3DLodDefinition> lodDefs )
    {
      if ( reader.ReadByte() == 0 )
        return;

      foreach ( var lodDef in lodDefs )
        lodDef.Index = reader.ReadByte();
    }

    private void ReadIsLastLodProperty( BinaryReader reader, List<S3DLodDefinition> lodDefs )
    {
      if ( reader.ReadByte() == 0 )
        return;

      foreach ( var lodDef in lodDefs )
        lodDef.IsLastLodUpToInfinity = reader.ReadBoolean();
    }

  }

}
