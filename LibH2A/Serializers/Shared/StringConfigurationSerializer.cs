using System;
using System.Collections.Generic;
using System.IO;
using Saber3D.Common;
using static Saber3D.Assertions;

namespace Saber3D.Serializers.Shared
{

  public class StringConfigurationSerializer<T> : ConfigurationSerializerBase<T>
    where T : class, new()
  {

    protected override void OnDeserialize( BinaryReader reader, T obj )
    {
      var stringValue = reader.ReadPascalString32();
      var stringReader = new StringReader( stringValue );

      DeserializeFromString( stringReader, obj );
    }

    public T DeserializeFromString( StringReader reader, T obj = null )
    {
      if ( obj is null )
        obj = new T();

      while ( true )
      {
        if ( !ReadProperty( obj, reader ) )
          break;
      }

      return obj;
    }

    private bool ReadProperty( T obj, StringReader reader )
    {
      var line = reader.ReadLine();
      if ( string.IsNullOrEmpty( line ) )
        return false;

      line = line.Trim();

      if ( line == "}" )
        return false;

      var parts = line.Split( new[] { ' ', '=' }, StringSplitOptions.RemoveEmptyEntries );

      var propertyName = parts[ 0 ];
      var value = parts[ 1 ];

      object parsedValue;
      if ( value == "{" )
        parsedValue = ParseNestedValue( propertyName, reader );
      else
        parsedValue = ParseValue( reader, propertyName, value );

      SetPropertyValue( obj, propertyName, parsedValue );

      return true;
    }

    private object ParseValue( StringReader reader, string propertyName, string value )
    {
      var property = GetProperty( propertyName );
      var propertyType = property.PropertyType;

      if ( propertyType == typeof( int ) )
        return int.Parse( value );
      else if ( propertyType == typeof( float ) )
        return float.Parse( value );
      else if ( propertyType == typeof( bool ) )
        return bool.Parse( value );
      else if ( propertyType == typeof( string ) )
        return value;
      else if ( propertyType == typeof( float[] ) )
      {
        Assert( value == "[", "Expected start of array." );

        var arr = new List<float>();
        value = reader.ReadLine().Trim().Replace( ",", "" );
        while ( value != "]" )
        {
          arr.Add( float.Parse( value ) );
          value = reader.ReadLine().Trim().Replace( ",", "" );
        }

        return arr.ToArray();
      }
      else
        return FailReturn<object>( $"Unable to parse value for configuration: {value}" );
    }

    private object ParseNestedValue( string propertyName, StringReader reader )
    {
      // TODO: This reflection is slow and ugly.

      var property = GetProperty( propertyName );

      // Create Serializer
      var serializerType = typeof( StringConfigurationSerializer<> ).MakeGenericType( property.PropertyType );
      var deserializeMethod = serializerType.GetMethod( "DeserializeFromString" );
      var serializer = Activator.CreateInstance( serializerType );

      return deserializeMethod.Invoke( serializer, new[] { reader, null } );
    }

  }

}
