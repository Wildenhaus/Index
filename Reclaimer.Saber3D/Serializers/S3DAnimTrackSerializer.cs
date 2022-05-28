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
      var unk_01 = reader.ReadUInt32();
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
      //// TODO: No idea what this is. Maybe a bitfield? RTTI says its a Vector<Vector<short,8>>
      //// This is similar to the 0x0300000007 in S3DObjectGeometryUnshared
      //// Might be a bitfield? Maybe length is (0xC * ObjAnimList.Count)?
      //var unk_01 = reader.ReadUInt32();
      //var unk_02 = reader.ReadByte();
      //Assert( unk_01 == 0x03 );
      //Assert( unk_02 == 0x05 );
      //for ( var i = 0; i < 0xC; i++ )
      //  reader.ReadByte();
      throw new NotImplementedException();
    }

    private void ReadRootAnimProperty( BinaryReader reader, S3DAnimTrack animTrack )
    {
      var serializer = new S3DAnimRootedSerializer();
      animTrack.RootAnim = serializer.Deserialize( reader );
    }

  }

}
