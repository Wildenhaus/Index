using System;
using System.Collections.Generic;
using System.IO;
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
      ReadOnReadAnimExtraProperty( reader, template );
      ReadBoundingBoxProperty( reader, template );
      ReadLodDefinitionProperty( reader, template );
      ReadTexListProperty( reader, template );
      //ReadGeometryMngProperty( reader, template );
      //ReadExternDataProperty( reader, template );
    }

    #endregion

    #region Private Methods

    private void ReadPropertyCount( BinaryReader reader, S3DTemplate template )
    {
      /* These values are immediately after the TPL1 signature.
       * uint32 PropertyCount
       *   - Max index of the properties.
       * uint16 PropertyFlags
       *   - A BitField denoting which properties are present.
       */

      template.PropertyCount = reader.ReadUInt32();
      template.PropertyFlags = ( TemplatePropertyFlags ) reader.ReadUInt16();
    }

    private void ReadNameProperty( BinaryReader reader, S3DTemplate template )
    {
      /* This is the name of the Template.
       * Pascal String (int32)
       */
      if ( !template.PropertyFlags.HasFlag( TemplatePropertyFlags.Name ) )
        return;

      template.Name = reader.ReadPascalString32();
    }

    private void ReadNameClassProperty( BinaryReader reader, S3DTemplate template )
    {
      /* Not sure what this is. Haven't encountered any files with it yet.
       * RTTI states that it's a Pascal String (int32)
       */
      if ( !template.PropertyFlags.HasFlag( TemplatePropertyFlags.NameClass ) )
        return;

      template.NameClass = reader.ReadPascalString32();
    }

    private void ReadStateProperty( BinaryReader reader, S3DTemplate template )
    {
      // TODO
      /* Not sure what this is. It is present in a majority of files.
       * 48 bits in length. Possibly bitfield flags.
       */
      if ( !template.PropertyFlags.HasFlag( TemplatePropertyFlags.State ) )
        return;

      var unk0 = reader.ReadUInt16();
      var unk1 = reader.ReadUInt32();
    }

    private void ReadAffixesProperty( BinaryReader reader, S3DTemplate template )
    {
      // TODO
      /* A bunch of export/attribute strings. Not sure what they're used for.
       * RTTI says this uses a special string serializer.
       * There seems to be a delimiter between each string. It might be deserialized to a list.
       * Represented as a Pascal String (int32)
       */
      if ( !template.PropertyFlags.HasFlag( TemplatePropertyFlags.Affixes ) )
        return;

      template.Affixes = reader.ReadPascalString32();
    }

    private void ReadPsProperty( BinaryReader reader, S3DTemplate template )
    {
      /* Some sort of scripting tied to the Template.
       * Most of the time this is just a one-line script denoting a base type.
       * RTTI says it uses a special serializer.
       * Represented as a Pascal String (int32)
       */
      if ( !template.PropertyFlags.HasFlag( TemplatePropertyFlags.PS ) )
        return;

      template.PS = reader.ReadPascalString32();
    }

    private void ReadSkinProperty( BinaryReader reader, S3DTemplate template )
    {
      // TODO
      /* Not sure what this is. Maybe skinning data, but tied to the whole template.
       * It's a list of 4x4 matrices. Sometimes it cuts off early.
       */
      if ( !template.PropertyFlags.HasFlag( TemplatePropertyFlags.Skin ) )
        return;

      var count = reader.ReadUInt32();
      var unk_0 = reader.ReadUInt16();
      var unk_1 = reader.ReadByte();

      // Sometimes the count is positive, but the data ends immediately after.
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
      /* Animation Tracks for the Template.
       */
      if ( !template.PropertyFlags.HasFlag( TemplatePropertyFlags.TrackAnim ) )
        return;

      var serializer = new S3DAnimTrackSerializer();
      template.AnimTrack = serializer.Deserialize( reader );
    }

    private void ReadOnReadAnimExtraProperty( BinaryReader reader, S3DTemplate template )
    {
      /* Not sure what this is. No files seem to use it.
       * Keeping it here so we can throw an exception if it ever pops up.
       */
      if ( !template.PropertyFlags.HasFlag( TemplatePropertyFlags.OnReadAnimExtra ) )
        return;

      throw new NotImplementedException();
    }

    private void ReadBoundingBoxProperty( BinaryReader reader, S3DTemplate template )
    {
      /* Bounding box for the whole Template.
       */
      if ( !template.PropertyFlags.HasFlag( TemplatePropertyFlags.BoundingBox ) )
        return;

      template.BoundingBox = new M3DBox( reader.ReadVector3(), reader.ReadVector3() );
    }

    private void ReadLodDefinitionProperty( BinaryReader reader, S3DTemplate template )
    {
      /* Level-od-detail definitions for the Template.
       */
      if ( !template.PropertyFlags.HasFlag( TemplatePropertyFlags.LodDefinition ) )
        return;

      var lodDefSerializer = new S3DLodDefinitionSerializer();
      template.LodDefinitions = lodDefSerializer.Deserialize( reader );
    }

    private void ReadTexListProperty( BinaryReader reader, S3DTemplate template )
    {
      /* A list of common textures that the Template references.
       * Not sure how it's used.
       * Its a list of Pascal Strings (int16)
       */
      if ( !template.PropertyFlags.HasFlag( TemplatePropertyFlags.TextureList ) )
        return;

      var count = reader.ReadUInt32();
      var unk0 = reader.ReadUInt16(); // Always 0?
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
      /* Geometry (Multi-Node Graph?) Data
       * Contains most of the model info.
       */
      if ( !template.PropertyFlags.HasFlag( TemplatePropertyFlags.GeometryMNG ) )
        return;

      var geometryGraphSerializer = new S3DGeometryGraphSerializer();
      template.GeometryGraph = geometryGraphSerializer.Deserialize( reader );
    }

    private void ReadExternDataProperty( BinaryReader reader, S3DTemplate template )
    {
      // TODO
      /* External Data. Not quite sure what it's used for.
       * Not being deserialized yet.
       */
      if ( !template.PropertyFlags.HasFlag( TemplatePropertyFlags.ExternData ) )
        return;

      throw new NotImplementedException();
    }

    #endregion

  }

}
