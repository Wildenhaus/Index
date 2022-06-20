namespace Saber3D.Serializers.Scripting
{

  public enum ScriptingTokenType
  {

    Unknown = 0,

    PropertyName,
    Assignment,
    Value,

    StartObject,
    EndObject,

    StartArray,
    EndArray,
  }

}
