using System;

namespace Saber3D.Files
{

  public class FileSignatureAttribute : Attribute
  {

    public string Signature { get; }

    public FileSignatureAttribute( string signature )
    {
      Signature = signature;
    }

    private byte[] GetByteSignature()
      => System.Text.Encoding.UTF8.GetBytes( Signature );

  }

}
