using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Saber3D.Common;
using Saber3D.Data;
using static Saber3D.Assertions;

namespace Saber3D.Serializers
{

  public class S3DTemplateSerializer : SerializerBase<S3DTemplate>
  {

    #region Constants

    private const uint SIGNATURE_TPL1 = 0x314C5054; //TPL1

    #endregion

    #region Constructor

    public S3DTemplateSerializer()
    {
    }

    #endregion

    #region Overrides

    protected override void OnDeserialize( BinaryReader reader, S3DTemplate template )
    {
      ReadSignature( reader, SIGNATURE_TPL1 );
      ReadPropertyCount( reader, template );

      ReadNameProperty( reader, template );
      ReadNameClassProperty( reader, template );
      ReadStateProperty( reader, template );
      ReadAffixesProperty( reader, template );
      ReadPsProperty( reader, template );
      ReadSkinProperty( reader, template );
      ReadTrackAnimProperty( reader, template );
      ReadBoundingBoxProperty( reader, template );
      ReadLodDefinitionProperty( reader, template );
      ReadTexListProperty( reader, template );
      ReadGeometryMngProperty( reader, template );
      ReadExternDataProperty( reader, template );
    }

    #endregion

    #region Private Methods

    private void ReadPropertyCount( BinaryReader reader, S3DTemplate template )
    {
      /* Property Count:
       * These values are immediately after the TPL1 signature.
       * uint32 PropertyCount
       *   - Likely the max index for properties.
       * uint16 PropertyFlags
       *   - A BitField denoting which properties are present.
       */

      template.PropertyCount = reader.ReadUInt32();
      template.PropertyFlags = ( TemplatePropertyFlags ) reader.ReadUInt16();

      foreach ( var flag in Enum.GetValues( typeof( TemplatePropertyFlags ) ).Cast<TemplatePropertyFlags>() )
        if ( template.PropertyFlags.HasFlag( flag ) )
          Console.WriteLine( "  {0}", flag );

    }

    private void ReadNameProperty( BinaryReader reader, S3DTemplate template )
    {
      if ( !template.PropertyFlags.HasFlag( TemplatePropertyFlags.Name ) )
        return;

      template.Name = reader.ReadPascalString32();
    }

    private void ReadNameClassProperty( BinaryReader reader, S3DTemplate template )
    {
      if ( !template.PropertyFlags.HasFlag( TemplatePropertyFlags.NameClass ) )
        return;

      template.NameClass = reader.ReadPascalString32();
    }

    private void ReadStateProperty( BinaryReader reader, S3DTemplate template )
    {
      if ( !template.PropertyFlags.HasFlag( TemplatePropertyFlags.State ) )
        return;

      var unk0 = reader.ReadUInt16();
      var unk1 = reader.ReadUInt32();
    }

    private void ReadAffixesProperty( BinaryReader reader, S3DTemplate template )
    {
      if ( !template.PropertyFlags.HasFlag( TemplatePropertyFlags.Affixes ) )
        return;

      template.Affixes = reader.ReadPascalString32();
    }

    private void ReadPsProperty( BinaryReader reader, S3DTemplate template )
    {
      if ( !template.PropertyFlags.HasFlag( TemplatePropertyFlags.PS ) )
        return;

      template.PS = reader.ReadPascalString32();
    }

    private void ReadSkinProperty( BinaryReader reader, S3DTemplate template )
    {
      if ( !template.PropertyFlags.HasFlag( TemplatePropertyFlags.Skin ) )
        return;

      var count = reader.ReadUInt32();
      var unk_0 = reader.ReadUInt16();
      var unk_1 = reader.ReadByte();

      var endFlag = reader.ReadUInt16();
      if ( endFlag != 0xFFFF )
      {
        var endOffset = reader.ReadUInt32();

        for ( var i = 0; i < count; i++ )
          reader.ReadMatrix4x4();

        Assert( endOffset == reader.BaseStream.Position, "Invalid end position for TPL1 Skin property." );

        endFlag = reader.ReadUInt16();
      }

      Assert( endFlag == 0xFFFF, "Invalid read of TPL1.skin property" );
      Assert( reader.ReadUInt32() == reader.BaseStream.Position, "Invalid end position for TPL1 Skin property." );
    }

    private void ReadTrackAnimProperty( BinaryReader reader, S3DTemplate template )
    {
      if ( !template.PropertyFlags.HasFlag( TemplatePropertyFlags.TrackAnim ) )
        return;

      var serializer = new S3DAnimTrackSerializer();
      template.AnimTrack = serializer.Deserialize( reader );
    }

    //private void ReadOnReadAnimExtraProperty( BinaryReader reader, S3DTemplate template )
    //{
    //  if ( !template.PropertyFlags.HasFlag( TemplatePropertyFlags.OnReadAnimExtra ) )
    //    return;

    //  Fail( "Unread property: OnReadAnimExtra" );
    //  Console.WriteLine( "TPL OnReadAnimExtra Read" );
    //}

    private void ReadBoundingBoxProperty( BinaryReader reader, S3DTemplate template )
    {
      if ( !template.PropertyFlags.HasFlag( TemplatePropertyFlags.BoundingBox ) )
        return;

      template.BoundingBox = new M3DBox( reader.ReadVector3(), reader.ReadVector3() );
    }

    private void ReadLodDefinitionProperty( BinaryReader reader, S3DTemplate template )
    {
      if ( !template.PropertyFlags.HasFlag( TemplatePropertyFlags.LodDefinition ) )
        return;

      var lodDefSerializer = new S3DLodDefinitionSerializer();
      template.LodDefinitions = lodDefSerializer.Deserialize( reader );
    }

    private void ReadTexListProperty( BinaryReader reader, S3DTemplate template )
    {
      if ( !template.PropertyFlags.HasFlag( TemplatePropertyFlags.TextureList ) )
        return;

      var count = reader.ReadUInt32();
      var unk0 = reader.ReadUInt16();
      var endOffset = reader.ReadUInt32();

      template.TexList = new List<string>( ( int ) count );
      for ( var i = 0; i < count; i++ )
        template.TexList.Add( reader.ReadPascalString16() );

      var endMarker = reader.ReadUInt16();
      _ = reader.ReadUInt32(); // EndOffset, points to next data
      Assert( endMarker == 0xFFFF, "Invalid EndMarker for TexList Property." );
    }

    private void ReadGeometryMngProperty( BinaryReader reader, S3DTemplate template )
    {
      if ( !template.PropertyFlags.HasFlag( TemplatePropertyFlags.GeometryMNG ) )
        return;

      var geometryGraphSerializer = new S3DGeometryGraphSerializer();
      template.GeometryGraph = geometryGraphSerializer.Deserialize( reader );
    }

    private void ReadExternDataProperty( BinaryReader reader, S3DTemplate template )
    {
      if ( !template.PropertyFlags.HasFlag( TemplatePropertyFlags.ExternData ) )
        return;

      Fail( "Unread property: externData" );

    }

    #endregion

  }

}
