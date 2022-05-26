using System;
using System.Collections;
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

      var unk_01 = reader.ReadUInt16();
      var unk_02 = reader.ReadUInt16(); // Section Count?
      var unk_03 = reader.ReadUInt16();

      for ( var i = 0; i < objectCount; i++ )
        objects.Add( new S3DObject() );

      ReadIdProperty( reader, objects );
      ReadReadNameProperty( reader, objects );
      ReadStateProperty( reader, objects );
      ReadParentIdProperty( reader, objects );
      ReadNextIdProperty( reader, objects );
      ReadPrevIdProperty( reader, objects );
      ReadChildIdProperty( reader, objects );
      ReadAnimNumberProperty( reader, objects );
      ReadReadAffixesProperty( reader, objects );
      ReadMatrixLTProperty( reader, objects );
      ReadMatrixModelProperty( reader, objects );
      ReadGeomDataProperty( reader, objects );
      ReadUnkNamesProperty( reader, objects );
      ReadObbProperty( reader, objects );
      ReadNameProperty( reader, objects );
      ReadAffixesProperty( reader, objects );
    }

    private void ReadIdProperty( BinaryReader reader, List<S3DObject> objects )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      for ( var i = 0; i < objects.Count; i++ )
        objects[ i ].Id = reader.ReadInt16();
    }

    private void ReadReadNameProperty( BinaryReader reader, List<S3DObject> objects )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      for ( var i = 0; i < objects.Count; i++ )
        objects[ i ].ReadName = reader.ReadPascalString32();
    }

    private void ReadStateProperty( BinaryReader reader, List<S3DObject> objects )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      for ( var i = 0; i < objects.Count; i++ )
      {
        _ = reader.ReadUInt16();
        _ = reader.ReadUInt16();
        _ = reader.ReadUInt16();
        _ = reader.ReadByte();
      }
    }

    private void ReadParentIdProperty( BinaryReader reader, List<S3DObject> objects )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      for ( var i = 0; i < objects.Count; i++ )
        objects[ i ].ParentId = reader.ReadInt16();
    }

    private void ReadNextIdProperty( BinaryReader reader, List<S3DObject> objects )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      for ( var i = 0; i < objects.Count; i++ )
        objects[ i ].NextId = reader.ReadInt16();
    }

    private void ReadPrevIdProperty( BinaryReader reader, List<S3DObject> objects )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      for ( var i = 0; i < objects.Count; i++ )
        objects[ i ].PrevId = reader.ReadInt16();
    }

    private void ReadChildIdProperty( BinaryReader reader, List<S3DObject> objects )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      for ( var i = 0; i < objects.Count; i++ )
        objects[ i ].ChildId = reader.ReadInt16();
    }

    private void ReadAnimNumberProperty( BinaryReader reader, List<S3DObject> objects )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      for ( var i = 0; i < objects.Count; i++ )
        objects[ i ].AnimNumber = reader.ReadInt16();
    }

    private void ReadReadAffixesProperty( BinaryReader reader, List<S3DObject> objects )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      for ( var i = 0; i < objects.Count; i++ )
        objects[ i ].ReadAffixes = reader.ReadPascalString32();
    }

    private void ReadMatrixLTProperty( BinaryReader reader, List<S3DObject> objects )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      for ( var i = 0; i < objects.Count; i++ )
        objects[ i ].MatrixLT = reader.ReadMatrix4x4();
    }

    private void ReadMatrixModelProperty( BinaryReader reader, List<S3DObject> objects )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      for ( var i = 0; i < objects.Count; i++ )
        objects[ i ].MatrixModel = reader.ReadMatrix4x4();
    }

    private void ReadGeomDataProperty( BinaryReader reader, List<S3DObject> objects )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      var objFlagLen = ( int ) Math.Ceiling( objects.Count / 8f );
      var objFlagData = new byte[ objFlagLen ];
      reader.Read( objFlagData, 0, objFlagLen );

      var objFlags = new BitArray( objFlagData );

      for ( var i = 0; i < objects.Count; i++ )
      {
        if ( objFlags[ i ] )
        {
          var unk_01 = reader.ReadUInt32(); // 0x00000003
          var unk_02 = reader.ReadByte(); //0x7
          Assert( unk_01 == 0x3, "3 val not found" );
          Assert( unk_02 == 0x7, "7 val not found" );

          var geomData = new S3DObjectGeometryUnshared();
          geomData.SplitIndex = reader.ReadUInt32();
          geomData.NumSplits = reader.ReadUInt32();
          geomData.BoundingBox = new M3DBox( reader.ReadVector3(), reader.ReadVector3() );
        }
      }

      Assert( reader.PeekByte() != 0x3, "Still more objGEOM_UNSHARED data" );
    }

    private void ReadUnkNamesProperty( BinaryReader reader, List<S3DObject> objects )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      for ( var i = 0; i < objects.Count; i++ )
        objects[ i ].UnkName = reader.ReadPascalString32();
    }

    private void ReadObbProperty( BinaryReader reader, List<S3DObject> objects )
    {
      // TODO: Move this into M3DOBB serializer/data class
      // TODO: This seems to be all zeroes

      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      for ( var i = 0; i < objects.Count; i++ )
        for ( var j = 0; j < 60; j++ )
          reader.ReadByte();
    }

    private void ReadNameProperty( BinaryReader reader, List<S3DObject> objects )
    {
      // TODO: Move this into M3DOBB serializer/data class
      // TODO: This seems to be all zeroes

      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      for ( var i = 0; i < objects.Count; i++ )
        objects[ i ].Name = reader.ReadPascalString16();
    }

    private void ReadAffixesProperty( BinaryReader reader, List<S3DObject> objects )
    {
      // TODO: Move this into M3DOBB serializer/data class
      // TODO: This seems to be all zeroes

      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      for ( var i = 0; i < objects.Count; i++ )
        objects[ i ].Affixes = reader.ReadPascalString16();
    }

  }

}
