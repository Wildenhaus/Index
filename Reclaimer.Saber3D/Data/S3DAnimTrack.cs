using System.Collections.Generic;

namespace Saber3D.Data
{

  public class S3DAnimTrack
  {

    public List<S3DAnimSeq> SeqList { get; set; }
    public List<S3DObjectAnimation> ObjAnimList { get; set; }
    public List<short> ObjMapList { get; set; }
    public S3DAnimRooted RootAnim { get; set; }

  }

}
