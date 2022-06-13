using System;

namespace Saber3D.Files
{

  public class FileExtensionAttribute : Attribute
  {

    public string[] FileExtensions { get; }

    public FileExtensionAttribute( params string[] fileExtensions )
    {
      FileExtensions = fileExtensions;
    }

  }

}
