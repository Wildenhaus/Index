using System;
using System.Collections.Generic;

namespace Saber3D.Data
{

  public class S3DTemplate
  {

    public uint PropertyCount { get; set; }
    public TemplatePropertyFlags PropertyFlags { get; set; }

    public string Name { get; set; }
    public string NameClass { get; set; }
    public string Affixes { get; set; }
    public string PS { get; set; }
    // Skin
    public S3DAnimTrack AnimTrack { get; set; }
    public M3DBox BoundingBox { get; set; }
    public List<S3DLodDefinition> LodDefinitions { get; set; }
    public List<string> TexList { get; set; }
    public S3DGeometryGraph GeometryGraph { get; set; }
  }

  [Flags]
  public enum TemplatePropertyFlags : ushort
  {
    Name = 1 << 0,
    NameClass = 1 << 1,
    State = 1 << 2,
    Affixes = 1 << 3,
    PS = 1 << 4,
    Skin = 1 << 5,
    TrackAnim = 1 << 6,
    OnReadAnimExtra = 1 << 7,
    BoundingBox = 1 << 8,
    LodDefinition = 1 << 9,
    TextureList = 1 << 10,
    GeometryMNG = 1 << 11,
    ExternData = 1 << 12
  }

}
