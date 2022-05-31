using System.Collections.Generic;
using System.IO;
using Saber3D.Common;
using Saber3D.Data;

namespace Saber3D.Serializers
{

  public class S3DAnimSeqSerializer : SerializerBase<List<S3DAnimSeq>>
  {

    protected override void OnDeserialize( BinaryReader reader, List<S3DAnimSeq> seqList )
    {
      var count = reader.ReadUInt32();
      var propertyCount = reader.ReadUInt32();

      for ( var i = 0; i < count; i++ )
        seqList.Add( new S3DAnimSeq() );

      ReadNameProperty( reader, seqList );
      ReadLayerIdProperty( reader, seqList );
      ReadStartFrameProperty( reader, seqList );
      ReadEndFrameProperty( reader, seqList );
      ReadOffsetFrameProperty( reader, seqList );
      ReadLenFrameProperty( reader, seqList );
      ReadTimeSecProperty( reader, seqList );
      ReadActionFramesProperty( reader, seqList );
      ReadBoundingBoxProperty( reader, seqList );
    }

    private void ReadNameProperty( BinaryReader reader, List<S3DAnimSeq> seqList )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      for ( var i = 0; i < seqList.Count; i++ )
        seqList[ i ].Name = reader.ReadPascalString32();
    }

    private void ReadLayerIdProperty( BinaryReader reader, List<S3DAnimSeq> seqList )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      for ( var i = 0; i < seqList.Count; i++ )
        seqList[ i ].LayerId = reader.ReadUInt32();
    }

    private void ReadStartFrameProperty( BinaryReader reader, List<S3DAnimSeq> seqList )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      for ( var i = 0; i < seqList.Count; i++ )
        seqList[ i ].StartFrame = reader.ReadSingle();
    }

    private void ReadEndFrameProperty( BinaryReader reader, List<S3DAnimSeq> seqList )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      for ( var i = 0; i < seqList.Count; i++ )
        seqList[ i ].EndFrame = reader.ReadSingle();
    }

    private void ReadOffsetFrameProperty( BinaryReader reader, List<S3DAnimSeq> seqList )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      for ( var i = 0; i < seqList.Count; i++ )
        seqList[ i ].OffsetFrame = reader.ReadSingle();
    }

    private void ReadLenFrameProperty( BinaryReader reader, List<S3DAnimSeq> seqList )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      for ( var i = 0; i < seqList.Count; i++ )
        seqList[ i ].LenFrame = reader.ReadSingle();
    }

    private void ReadTimeSecProperty( BinaryReader reader, List<S3DAnimSeq> seqList )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      for ( var i = 0; i < seqList.Count; i++ )
        seqList[ i ].TimeSec = reader.ReadSingle();
    }

    private void ReadActionFramesProperty( BinaryReader reader, List<S3DAnimSeq> seqList )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      var serializer = new S3DActionFrameSerializer();
      for ( var i = 0; i < seqList.Count; i++ )
        seqList[ i ].ActionFrames = serializer.Deserialize( reader );
    }

    private void ReadBoundingBoxProperty( BinaryReader reader, List<S3DAnimSeq> seqList )
    {
      // Read Sentinel
      if ( reader.ReadByte() == 0 )
        return;

      for ( var i = 0; i < seqList.Count; i++ )
        seqList[ i ].BoundingBox = new M3DBox( reader.ReadVector3(), reader.ReadVector3() );
    }

  }

}
