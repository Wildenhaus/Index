namespace Saber3D.Data
{

  public class S3DGeometryBuffer
  {

    public S3DGeometryBufferInfo BufferInfo { get; set; }
    public ushort ElementSize { get; set; }
    public uint BufferLength { get; set; }

    public long StartOffset { get; set; }
    public long EndOffset { get; set; }

  }

  public struct S3DGeometryBufferInfo
  {
    public ushort Unk_01; // TODO
    public byte Unk_02; // TODO
    public S3DGeometryBufferType BufferType;
    public byte Unk_04; // TODO
    public byte Unk_05; // TODO
    public byte Unk_06; // TODO
    public byte Unk_07; // TODO
    public byte Unk_08; // TODO
    public byte Unk_09; // TODO
  }

  public enum S3DGeometryBufferType : byte
  {
    // TODO
    /* I almost want to say these are bitflags, but I can't quite
     * figure out how they are meant to work.
     * 
     * Ex:
     *  0x00 - 00000000 Face
     *  0x02 - 00000010 Unk
     *  0x04 - 00000100 Unk
     *  0x0C - 00001100 StaticVert
     *  0x0F - 00001111 SkinnedVert
     *  0x10 - 00010000 UVs
     *  0x30 - 00110000 UVs 2
     *  0x70 - 01110000 UVs 3
     *  0xF0 - 11110000 UVs 4
     */

    Face = 0x00,
    Unk_02 = 0x02,
    Unk_04 = 0x04,
    StaticVert = 0x0C,
    SkinnedVert = 0x0F,

    // The 4 high bits represent how many UVs are present
    UV1 = 0x10,
    UV2 = 0x30,
    UV3 = 0x70,
    UV4 = 0xF0
  }

}
