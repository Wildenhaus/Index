using System;

namespace Saber3D.Data.Materials
{

  public class MaterialPropertyAttribute : Attribute
  {

    public readonly string PropertyName;

    public MaterialPropertyAttribute( string propertyName )
    {
      PropertyName = propertyName;
    }

  }

}
