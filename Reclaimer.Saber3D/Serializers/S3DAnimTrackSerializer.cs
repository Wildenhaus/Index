using System.Collections.Generic;
using System.IO;
using Saber3D.Data;

namespace Saber3D.Serializers
{

  public class S3DAnimTrackSerializer : SerializerBase<S3DAnimTrack>
  {

    protected override void OnDeserialize( BinaryReader reader, S3DAnimTrack animTrack )
    {
      var unk_01 = reader.ReadUInt32();
      var unk_02 = reader.ReadByte();

      ReadAnimSeqProperty( reader, animTrack );
      ReadObjAnimListProperty( reader, animTrack );
      ReadObjMapListProperty( reader, animTrack );
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
      animTrack.ObjMapList = new List<short>();
      for ( var i = 0; i < 8; i++ )
        animTrack.ObjMapList.Add( reader.ReadInt16() );
    }

    private void ReadRootAnimProperty( BinaryReader reader, S3DAnimTrack animTrack )
    {
      var serializer = new S3DAnimRootedSerializer();
      animTrack.RootAnim = serializer.Deserialize( reader );
    }

  }

}
