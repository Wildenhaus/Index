using System.Collections.Generic;
using System.IO;
using Saber3D.Common;
using Saber3D.Data;
using static Saber3D.Assertions;

namespace Saber3D.Serializers
{

  public class S3DObjectSerializer : SerializerBase<List<S3DObject>>
  {

    protected override void OnDeserialize( BinaryReader reader, List<S3DObject> objects )
    {
      var objectCount = reader.ReadUInt16();

      var unk_01 = reader.ReadUInt16(); // TODO: always 0, padding?
      var propertyCount = reader.ReadUInt16();
      var unk_03 = reader.ReadUInt16(); // TODO: always 0, padding?

      for ( var i = 0; i < objectCount; i++ )
        objects.Add( new S3DObject() );

      if ( propertyCount > 0 )
        ReadIdProperty( reader, objects );
      if ( propertyCount > 1 )
        ReadReadNameProperty( reader, objects );
      if ( propertyCount > 2 )
        ReadStateProperty( reader, objects );
      if ( propertyCount > 3 )
        ReadParentIdProperty( reader, objects );
      if ( propertyCount > 4 )
        ReadNextIdProperty( reader, objects );
      if ( propertyCount > 5 )
        ReadPrevIdProperty( reader, objects );
      if ( propertyCount > 6 )
        ReadChildIdProperty( reader, objects );
      if ( propertyCount > 7 )
        ReadAnimNumberProperty( reader, objects );
      if ( propertyCount > 8 )
        ReadReadAffixesProperty( reader, objects );
      if ( propertyCount > 9 )
        ReadMatrixLTProperty( reader, objects );
      if ( propertyCount > 10 )
        ReadMatrixModelProperty( reader, objects );
      if ( propertyCount > 11 )
        ReadGeomDataProperty( reader, objects );
      if ( propertyCount > 12 )
        ReadUnkNamesProperty( reader, objects );
      if ( propertyCount > 13 )
        ReadObbProperty( reader, objects );
      if ( propertyCount > 14 )
        ReadNameProperty( reader, objects );
      if ( propertyCount > 15 )
        ReadAffixesProperty( reader, objects );
    }

    private void ReadIdProperty( BinaryReader reader, List<S3DObject> objects )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      foreach ( var obj in objects )
        obj.Id = reader.ReadInt16();
    }

    private void ReadReadNameProperty( BinaryReader reader, List<S3DObject> objects )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      foreach ( var obj in objects )
        obj.ReadName = reader.ReadPascalString32();
    }

    private void ReadStateProperty( BinaryReader reader, List<S3DObject> objects )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      for ( var i = 0; i < objects.Count; i++ )
      {
        _ = reader.ReadUInt16(); // TODO: Unk
        _ = reader.ReadUInt16(); // TODO: Unk
        _ = reader.ReadUInt16(); // TODO: Unk
        _ = reader.ReadByte();   // TODO: Unk
      }
    }

    private void ReadParentIdProperty( BinaryReader reader, List<S3DObject> objects )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      foreach ( var obj in objects )
        obj.ParentId = reader.ReadInt16();
    }

    private void ReadNextIdProperty( BinaryReader reader, List<S3DObject> objects )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      foreach ( var obj in objects )
        obj.NextId = reader.ReadInt16();
    }

    private void ReadPrevIdProperty( BinaryReader reader, List<S3DObject> objects )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      foreach ( var obj in objects )
        obj.PrevId = reader.ReadInt16();
    }

    private void ReadChildIdProperty( BinaryReader reader, List<S3DObject> objects )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      foreach ( var obj in objects )
        obj.ChildId = reader.ReadInt16();
    }

    private void ReadAnimNumberProperty( BinaryReader reader, List<S3DObject> objects )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      foreach ( var obj in objects )
        obj.AnimNumber = reader.ReadInt16();
    }

    private void ReadReadAffixesProperty( BinaryReader reader, List<S3DObject> objects )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      foreach ( var obj in objects )
        obj.ReadAffixes = reader.ReadPascalString32();
    }

    private void ReadMatrixLTProperty( BinaryReader reader, List<S3DObject> objects )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      foreach ( var obj in objects )
        obj.MatrixLT = reader.ReadMatrix4x4();
    }

    private void ReadMatrixModelProperty( BinaryReader reader, List<S3DObject> objects )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      foreach ( var obj in objects )
        obj.MatrixModel = reader.ReadMatrix4x4();
    }

    private void ReadGeomDataProperty( BinaryReader reader, List<S3DObject> objects )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      var objFlags = reader.ReadBitArray( objects.Count );

      for ( var i = 0; i < objects.Count; i++ )
      {
        if ( objFlags[ i ] )
        {
          var unk_01 = reader.ReadUInt32(); // TODO: 0x00000003
          var unk_02 = reader.ReadByte(); // TODO: 0x7
          Assert( unk_01 == 0x3, "3 val not found" );
          Assert( unk_02 == 0x7, "7 val not found" );

          var geomData = new S3DObjectGeometryUnshared();
          geomData.SplitIndex = reader.ReadUInt32();
          geomData.NumSplits = reader.ReadUInt32();
          geomData.BoundingBox = new M3DBox( reader.ReadVector3(), reader.ReadVector3() );

          objects[ i ].GeomData = geomData;
        }
      }

      Assert( reader.PeekByte() != 0x3, "Still more objGEOM_UNSHARED data" );
    }

    private void ReadUnkNamesProperty( BinaryReader reader, List<S3DObject> objects )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      foreach ( var obj in objects )
        obj.UnkName = reader.ReadPascalString32();
    }

    private void ReadObbProperty( BinaryReader reader, List<S3DObject> objects )
    {
      // TODO: Move this into M3DOBB serializer/data class
      // TODO: This seems to be all zeroes

      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      foreach ( var obj in objects )
        for ( var j = 0; j < 60; j++ )
          reader.ReadByte();
    }

    private void ReadNameProperty( BinaryReader reader, List<S3DObject> objects )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      foreach ( var obj in objects )
        obj.Name = reader.ReadPascalString16();
    }

    private void ReadAffixesProperty( BinaryReader reader, List<S3DObject> objects )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      foreach ( var obj in objects )
        obj.Affixes = reader.ReadPascalString16();
    }

  }

}
