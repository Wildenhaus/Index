using System.Collections.Generic;
using System.IO;
using Saber3D.Common;
using Saber3D.Data;
using static Saber3D.Assertions;

namespace Saber3D.Serializers
{

  public class S3DSceneSerializer : SerializerBase<S3DScene>
  {

    #region Constants

    private const uint SIGNATURE_SCN1 = 0x314E4353; //SCN1

    #endregion

    protected override void OnDeserialize( BinaryReader reader, S3DScene scene )
    {
      ReadSignature( reader, SIGNATURE_SCN1 );
      ReadPropertyCount( reader, scene );

      ReadTextureListProperty( reader, scene );
      ReadPsProperty( reader, scene );
      ReadInstMaterialInfoListProperty( reader, scene );
    }

    private void ReadPropertyCount( BinaryReader reader, S3DScene scene )
    {
      /* These values are immediately after the SCN1 signature.
       * uint32 PropertyCount
       *   - Max index of the properties.
       * uint16 PropertyFlags
       *   - A BitField denoting which properties are present.
       */

      scene.PropertyCount = reader.ReadUInt32();
      scene.PropertyFlags = reader.ReadBitArray( ( int ) scene.PropertyCount );
    }

    private void ReadTextureListProperty( BinaryReader reader, S3DScene scene )
    {
      /* A list of common textures that the Scene references.
       * Not sure how it's used.
       * Its a list of Pascal Strings (int16)
       */
      if ( !scene.PropertyFlags[ 0 ] )
        return;

      var count = reader.ReadUInt32();
      var unk0 = reader.ReadUInt16(); // TODO: Always 0?
      var endOffset = reader.ReadUInt32();

      scene.TextureList = new List<string>( ( int ) count );
      for ( var i = 0; i < count; i++ )
        scene.TextureList.Add( reader.ReadPascalString16() );

      var endMarker = reader.ReadUInt16();
      endOffset = reader.ReadUInt32(); // EndOffset, points to next data
      Assert( endMarker == 0xFFFF, "Invalid EndMarker for TexList Property." );
      Assert( reader.BaseStream.Position == endOffset, "Invalid EndOffset for TexList Property." );
    }

    private void ReadPsProperty( BinaryReader reader, S3DScene scene )
    {
      /* Some sort of scripting tied to the Scene.
       * Most of the time this is just a one-line script denoting a base type.
       * RTTI says it uses a special serializer.
       * Represented as a Pascal String (int32)
       */
      if ( !scene.PropertyFlags[ 1 ] )
        return;

      scene.PS = reader.ReadPascalString32();
    }

    private void ReadInstMaterialInfoListProperty( BinaryReader reader, S3DScene scene )
    {
      /* This seems to be some sort of scripting that can set custom properties on each
       * of the scene's TPLs or materials.
       * The first int32 is the count.
       * There is an unknown byte after that.
       * Then it's a list of Tuple<string,string>:
       *    Name: PascalString32 (can be empty)
       *    PropertyLine: PascalString32
       */
      if ( !scene.PropertyFlags[ 2 ] )
        return;

      var count = reader.ReadUInt32();
      var unk_01 = reader.ReadByte();

      var list = scene.InstMaterialInfoList = new List<string>();
      for ( var i = 0; i < count; i++ )
      {
        var materialName = reader.ReadPascalString32();
        var propertyLine = reader.ReadPascalString32();

        var entry = string.Format( "{0}: {1}", materialName, propertyLine );
        list.Add( entry );
      }
    }

    private void ReadGeometryMngProperty( BinaryReader reader, S3DScene scene )
    {
      /* Geometry (Multi-Node Graph?) Data
       * Contains most of the model info.
       */
      if ( !scene.PropertyFlags[ 3 ] )
        return;

      var geometryGraphSerializer = new S3DGeometryGraphSerializer();
      scene.GeometryGraph = geometryGraphSerializer.Deserialize( reader );
    }

  }

}
