using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Saber3D.Data.Shared;
using static Saber3D.Assertions;

namespace Saber3D.Serializers.Shared
{

  public abstract class ConfigurationSerializerBase<T> : SerializerBase<T>
    where T : class, new()
  {

    #region Data Members

    private static readonly Dictionary<string, PropertyInfo> _propertyLookup;

    #endregion

    #region Constructor

    static ConfigurationSerializerBase()
    {
      _propertyLookup = BuildPropertyLookup();
    }

    #endregion

    #region Private Methods

    protected static PropertyInfo GetProperty( string propertyName )
    {
      if ( !_propertyLookup.TryGetValue( propertyName, out var property ) )
        Fail( $"Unknown property for {typeof( T ).Name}: {propertyName}" );

      return property;
    }

    protected void SetPropertyValue( T obj, string propertyName, object value )
    {
      var property = GetProperty( propertyName );

      var objType = typeof( T );
      var propertyType = property.PropertyType;
      var valueType = value.GetType();

      Assert( property.DeclaringType == objType,
        $"Object-Property type mismatch for {propertyType.Name} {typeof( T ).Name}.{propertyName}: {valueType.Name}" );
      Assert( propertyType == valueType,
        $"Value-Property type mismatch for {propertyType.Name} {typeof( T ).Name}.{propertyName}: {valueType.Name}" );

      property.SetValue( obj, value );
    }

    protected DataType ReadDataType( BinaryReader reader )
    {
      var dataType = ( DataType ) reader.ReadUInt32();

      Assert( Enum.IsDefined( typeof( DataType ), dataType ),
        $"Unknown Configuration Property Data Type: {dataType:X}" );

      return dataType;
    }

    private static Dictionary<string, PropertyInfo> BuildPropertyLookup()
    {
      var lookup = new Dictionary<string, PropertyInfo>();

      var properties = typeof( T ).GetProperties();
      foreach ( var property in properties )
      {
        var attr = property.GetCustomAttribute<ConfigurationPropertyAttribute>();
        if ( attr is null )
          continue;

        lookup.Add( attr.PropertyName, property );
      }

      return lookup;
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
      NestedType = 7
    }

    #endregion

  }

}
