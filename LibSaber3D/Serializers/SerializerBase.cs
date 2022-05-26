using System.IO;

namespace Saber3D.Serializers
{

  public abstract class SerializerBase<T>
  {

    private readonly BinaryReader _reader;

    protected SerializerBase( BinaryReader reader )
    {
      _reader = reader;
    }

    public abstract T Deserialize();

  }

}
