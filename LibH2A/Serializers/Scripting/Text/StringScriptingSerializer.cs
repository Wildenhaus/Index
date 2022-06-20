using System;
using System.Collections.Generic;
using System.IO;
using Saber3D.Common;

namespace Saber3D.Serializers.Configurations
{

  public class StringScriptingSerializer<T> : TextScriptingSerializer<T>
    where T : class, new()
  {

    #region Data Members

    private readonly Dictionary<Type, IScriptingSerializer> _serializerCache
      = new Dictionary<Type, IScriptingSerializer>();

    #endregion

    #region Public Methods

    public T Deserialize( BinaryReader reader )
    {
      var obj = new T();

      OnDeserialize( reader, obj );

      return obj;
    }

    protected void OnDeserialize( BinaryReader reader, T obj )
    {
      var stringData = reader.ReadPascalString32();
      var stringReader = new StringReader( stringData );

      OnDeserialize( stringReader, obj );
    }

    #endregion

    #region Overrides

    protected override Dictionary<Type, IScriptingSerializer> GetSerializerCache()
      => _serializerCache;

    #endregion

  }

}
