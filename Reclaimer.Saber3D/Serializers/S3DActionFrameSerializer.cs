using System;
using System.Collections.Generic;
using System.IO;
using Saber3D.Data;

namespace Saber3D.Serializers
{

  public class S3DActionFrameSerializer : SerializerBase<List<S3DActionFrame>>
  {

    protected override void OnDeserialize( BinaryReader reader, List<S3DActionFrame> obj )
    {
      throw new NotImplementedException();
    }

  }

}
