using System;
using System.IO;
using Saber3D.Common;
using Saber3D.Data;

namespace Saber3D.Serializers
{

  public class S3DAnimTrackSerializer : SerializerBase<S3DAnimTrack>
  {

    protected override void OnDeserialize( BinaryReader reader, S3DAnimTrack animTrack )
    {
      var propertyCount = reader.ReadUInt32();
      var propertyFlags = reader.ReadBitArray( 4 );

      if ( propertyFlags[ 0 ] )
        ReadAnimSeqProperty( reader, animTrack );
      if ( propertyFlags[ 1 ] )
        ReadObjAnimListProperty( reader, animTrack );
      if ( propertyFlags[ 2 ] )
        ReadObjMapListProperty( reader, animTrack );
      if ( propertyFlags[ 3 ] )
        ReadRootAnimProperty( reader, animTrack );
    }

    private void ReadAnimSeqProperty( BinaryReader reader, S3DAnimTrack animTrack )
    {
      var serializer = new S3DAnimSeqSerializer();
      animTrack.SeqList = serializer.Deserialize( reader );
    }

    private void ReadObjAnimListProperty( BinaryReader reader, S3DAnimTrack animTrack )
    {
      var serializer = new S3DObjectAnimationSerializer();
      animTrack.ObjAnimList = serializer.Deserialize( reader );
    }

    private void ReadObjMapListProperty( BinaryReader reader, S3DAnimTrack animTrack )
    {
      // TODO: This is never used?
      // RTTI states this is a dsVECTOR<dsVECTOR<short,8>>
      throw new NotImplementedException();
    }

    private void ReadRootAnimProperty( BinaryReader reader, S3DAnimTrack animTrack )
    {
      var serializer = new S3DAnimRootedSerializer();
      animTrack.RootAnim = serializer.Deserialize( reader );
    }

  }

}
