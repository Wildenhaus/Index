using System;

namespace Saber3D.Data.Shared
{

  public class ConfigurationPropertyAttribute : Attribute
  {

    public readonly string PropertyName;

    public ConfigurationPropertyAttribute( string propertyName )
    {
      PropertyName = propertyName;
    }

  }

}
