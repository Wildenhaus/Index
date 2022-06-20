using System.IO;

namespace Saber3D.Serializers.Configurations
{

  public interface IBinaryScriptingSerializer : IScriptingSerializer
  {

    dynamic Deserialize( BinaryReader reader );

  }

  public interface IBinaryConfigurationSerializer<T> : IBinaryScriptingSerializer
    where T : class, new()
  {

    new T Deserialize( BinaryReader reader );

  }

}
