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

      if ( propertyCount > 0 )
        ReadIniTranslationProperty( reader, animList );
      if ( propertyCount > 1 )
        ReadPTranslationProperty( reader, animList );
      if ( propertyCount > 2 )
        ReadIniRotationProperty( reader, animList );
      if ( propertyCount > 3 )
        ReadPRotationProperty( reader, animList );
      if ( propertyCount > 4 )
        ReadIniScaleProperty( reader, animList );
      if ( propertyCount > 5 )
        ReadPScaleProperty( reader, animList );
      if ( propertyCount > 6 )
        ReadIniVisibilityProperty( reader, animList );
      if ( propertyCount > 7 )
        ReadPVisibilityProperty( reader, animList );
    }

    private void ReadIniTranslationProperty( BinaryReader reader, List<S3DObjectAnimation> animList )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      foreach ( var anim in animList )
        anim.IniTranslation = reader.ReadVector3();
    }

    private void ReadPTranslationProperty( BinaryReader reader, List<S3DObjectAnimation> animList )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      var propertyFlags = reader.ReadBitArray( animList.Count );

      var serializer = new M3DSplineSerializer();
      for ( var i = 0; i < animList.Count; i++ )
        if ( propertyFlags[ i ] )
          animList[ i ].PTranslation = serializer.Deserialize( reader );
    }

    private void ReadIniRotationProperty( BinaryReader reader, List<S3DObjectAnimation> animList )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      foreach ( var anim in animList )
        anim.IniRotation = reader.ReadVector4();
    }

    private void ReadPRotationProperty( BinaryReader reader, List<S3DObjectAnimation> animList )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      var propertyFlags = reader.ReadBitArray( animList.Count );

      var serializer = new M3DSplineSerializer();
      for ( var i = 0; i < animList.Count; i++ )
        if ( propertyFlags[ i ] )
          animList[ i ].PRotation = serializer.Deserialize( reader );
    }

    private void ReadIniScaleProperty( BinaryReader reader, List<S3DObjectAnimation> animList )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      foreach ( var anim in animList )
        anim.IniScale = reader.ReadVector3();
    }

    private void ReadPScaleProperty( BinaryReader reader, List<S3DObjectAnimation> animList )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      var propertyFlags = reader.ReadBitArray( animList.Count );

      var serializer = new M3DSplineSerializer();
      for ( var i = 0; i < animList.Count; i++ )
        if ( propertyFlags[ i ] )
          animList[ i ].PScale = serializer.Deserialize( reader );
    }

    private void ReadIniVisibilityProperty( BinaryReader reader, List<S3DObjectAnimation> animList )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      foreach ( var anim in animList )
        anim.IniVisibility = reader.ReadSingle();
    }

    private void ReadPVisibilityProperty( BinaryReader reader, List<S3DObjectAnimation> animList )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      var propertyFlags = reader.ReadBitArray( animList.Count );

      var serializer = new M3DSplineSerializer();
      for ( var i = 0; i < animList.Count; i++ )
        if ( propertyFlags[ i ] )
          animList[ i ].PVisibility = serializer.Deserialize( reader );
    }

  }
}
