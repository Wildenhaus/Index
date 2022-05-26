using System;
using System.Collections.Generic;
using System.IO;
using Saber3D.Common;
using Saber3D.Data;

namespace Saber3D.Serializers
{

  public class S3DObjectAnimationSerializer : SerializerBase<List<S3DObjectAnimation>>
  {

    protected override void OnDeserialize( BinaryReader reader, List<S3DObjectAnimation> animList )
    {
      var count = reader.ReadUInt32();
      var propertyCount = reader.ReadUInt32();

      for ( var i = 0; i < count; i++ )
        animList.Add( new S3DObjectAnimation() );

      ReadIniTranslationProperty( reader, animList );
      ReadPTranslationProperty( reader, animList );
      ReadIniRotationProperty( reader, animList );
      ReadPRotationProperty( reader, animList );
      ReadIniScaleProperty( reader, animList );
      ReadPScaleProperty( reader, animList );
      ReadIniVisibilityProperty( reader, animList );
      ReadPVisibilityProperty( reader, animList );
    }

    private void ReadIniTranslationProperty( BinaryReader reader, List<S3DObjectAnimation> animList )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      for ( var i = 0; i < animList.Count; i++ )
        animList[ i ].IniTranslation = reader.ReadVector3();
    }

    private void ReadPTranslationProperty( BinaryReader reader, List<S3DObjectAnimation> animList )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      throw new NotImplementedException();
    }

    private void ReadIniRotationProperty( BinaryReader reader, List<S3DObjectAnimation> animList )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      for ( var i = 0; i < animList.Count; i++ )
        animList[ i ].IniRotation = reader.ReadVector4();
    }

    private void ReadPRotationProperty( BinaryReader reader, List<S3DObjectAnimation> animList )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      throw new NotImplementedException();
    }

    private void ReadIniScaleProperty( BinaryReader reader, List<S3DObjectAnimation> animList )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      for ( var i = 0; i < animList.Count; i++ )
        animList[ i ].IniScale = reader.ReadVector3();
    }

    private void ReadPScaleProperty( BinaryReader reader, List<S3DObjectAnimation> animList )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      throw new NotImplementedException();
    }

    private void ReadIniVisibilityProperty( BinaryReader reader, List<S3DObjectAnimation> animList )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      for ( var i = 0; i < animList.Count; i++ )
        animList[ i ].IniVisibility = reader.ReadSingle();
    }

    private void ReadPVisibilityProperty( BinaryReader reader, List<S3DObjectAnimation> animList )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      for ( var i = 0; i < animList.Count; i++ )
        reader.BaseStream.Position += 0x2A;
    }

  }
}
