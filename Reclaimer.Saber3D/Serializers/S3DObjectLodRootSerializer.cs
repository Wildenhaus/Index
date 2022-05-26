using System;
using System.Collections.Generic;
using System.IO;
using Saber3D.Data;

namespace Saber3D.Serializers
{

  public class S3DObjectLodRootSerializer : SerializerBase<List<S3DObjectLodRoot>>
  {

    protected override void OnDeserialize( BinaryReader reader, List<S3DObjectLodRoot> lodRoots )
    {
      throw new NotImplementedException();
    }

  }

}
