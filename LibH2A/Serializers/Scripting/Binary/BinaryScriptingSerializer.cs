using System;
using System.IO;
using System.Text;
using Saber3D.Common;
using static Saber3D.Assertions;

namespace Saber3D.Serializers.Configurations
{

  public abstract class BinaryScriptingSerializer<T> : ScriptingSerializer<T>, IBinaryConfigurationSerializer<T>
    where T : class, new()
  {

    #region Public Methods

    public T Deserialize( BinaryReader reader )
    {
      var obj = new T();

      OnDeserialize( reader, obj );

      return obj;
    }

    dynamic IBinaryScriptingSerializer.Deserialize( BinaryReader reader )
      => Deserialize( reader );

    #endregion

    #region Abstract Methods

    protected abstract void ReadProperty( BinaryReader reader, T obj );

    #endregion

    #region Overrides

    protected void OnDeserialize( BinaryReader reader, T obj )
    {
      var propertyCount = reader.ReadUInt32();
      for ( var i = 0; i < propertyCount; i++ )
        ReadProperty( reader, obj );
    }

    protected override void OnDeserialize( Stream stream, T obj )
    {
      var reader = new BinaryReader( stream, Encoding.UTF8, true );
      OnDeserialize( reader, obj );
    }

    #endregion

    #region Protected Methods

    protected DataType ReadDataType( BinaryReader reader )
    {
      var dataType = ( DataType ) reader.ReadUInt32();

      if ( !Enum.IsDefined( typeof( DataType ), dataType ) )
        Fail( $"Unknown Configuration Property Data Type: {dataType:X}" );

      return dataType;
    }

    protected dynamic ReadValue( BinaryReader reader, DataType dataType, string propertyName )
    {
      dynamic readValue = null;

      switch ( dataType )
      {
        case DataType.Int:
          readValue = reader.ReadInt32();
          break;
        case DataType.Float:
          readValue = reader.ReadSingle();
          break;
        case DataType.Bool:
          readValue = reader.ReadBoolean();
          break;
        case DataType.String:
          readValue = reader.ReadPascalString32();
          break;
        case DataType.Array:
          readValue = ReadArray( reader, propertyName );
          break;
        case DataType.Class:
          readValue = ReadClass( reader, propertyName );
          break;
        default:
          Fail( $"Unhandled property: {dataType} {propertyName}" );
          break;
      }

      return readValue;
    }

    protected dynamic ReadArray( BinaryReader reader, string propertyName )
    {
      var count = reader.ReadUInt32();
      var array = new dynamic[ count ];

      for ( var i = 0; i < count; i++ )
      {
        var dataType = ReadDataType( reader );
        array[ i ] = ReadValue( reader, dataType, propertyName );
      }

      return array;
    }

    protected dynamic ReadClass( BinaryReader reader, string propertyName )
    {
      var property = GetProperty( propertyName );
      var serializer = GetSerializerForType( property.PropertyType ) as IBinaryScriptingSerializer;

      return serializer.Deserialize( reader );
    }

    #endregion

    #region Embedded Types

    protected enum DataType : uint
    {
      Int = 1,
      Float = 2,
      Bool = 3,
      String = 4,
      Array = 6, //{ Int(DataType), Value }
      Class = 7
    }

    #endregion

  }

}