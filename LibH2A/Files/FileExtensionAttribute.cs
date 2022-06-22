using System;

namespace Saber3D.Files
{

  [AttributeUsage( AttributeTargets.Class, AllowMultiple = true )]
  public class FileExtensionAttribute : Attribute
  {

    public string FileExtension { get; }

    public FileExtensionAttribute( string fileExtension )
    {
      FileExtension = fileExtension;
    }

  }

}
