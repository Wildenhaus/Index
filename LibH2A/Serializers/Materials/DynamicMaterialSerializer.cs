using System;
using System.IO;
using Saber3D.Common;
using static Saber3D.Assertions;

namespace Saber3D.Serializers.Materials
{

  public class DynamicMaterialSerializer<T> : MaterialSerializerBase<T>
    where T : class, new()
  {

    #region Overrides

    protected override void OnDeserialize( BinaryReader reader, T obj )
    {
      var propertyCount = reader.ReadUInt32();

      for ( var i = 0; i < propertyCount; i++ )
        ReadProperty( reader, obj );
    }

    #endregion

    #region Private Methods

    private void ReadProperty( BinaryReader reader, T obj )
    {
      var propertyName = reader.ReadPascalString32();
      var dataType = ReadDataType( reader );

      switch ( dataType )
      {
        case DataType.Int:
          SetPropertyValue( obj, propertyName, reader.ReadInt32() );
          break;
        case DataType.Float:
          SetPropertyValue( obj, propertyName, reader.ReadSingle() );
          break;
        case DataType.Bool:
          SetPropertyValue( obj, propertyName, reader.ReadBoolean() );
          break;
        case DataType.String:
          SetPropertyValue( obj, propertyName, reader.ReadPascalString32() );
          break;
        case DataType.Array:
        {
          var count = reader.ReadUInt32();
          var value = new float[ count ];
          for ( var i = 0; i < count; i++ )
          {
            // TODO: Arrays only seem to be used for floats, so it's hardcoded here.
            var arrayDataType = ( DataType ) reader.ReadUInt32();
            Assert( arrayDataType == DataType.Float, "Found a material array that isn't float!" );

            value[ i ] = reader.ReadSingle();
          }

          SetPropertyValue( obj, propertyName, value );
        }
        break;
        case DataType.NestedType:
          ReadNestedType( reader, obj, propertyName );
          break;
        default:
          Fail( $"Unhandled property: {dataType} {propertyName}" );
          break;
      }
    }

    protected void ReadNestedType( BinaryReader reader, T obj, string propertyName )
    {
      var property = GetProperty( propertyName );

      // Create Serializer
      var serializerType = typeof( DynamicMaterialSerializer<> ).MakeGenericType( property.PropertyType );
      var deserializeMethod = serializerType.GetMethod( "Deserialize" );
      var serializer = Activator.CreateInstance( serializerType );

      var value = deserializeMethod.Invoke( serializer, new[] { reader } );
      SetPropertyValue( obj, propertyName, value );
    }

    #endregion



  }

}
