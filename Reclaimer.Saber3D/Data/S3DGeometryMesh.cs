using System;

namespace Saber3D.Data
{

  public class S3DGeometryMesh
  {
    public ushort Unk_01 { get; set; } // TODO
    public S3DMeshFlags Flags { get; set; } // TODO: This is a guess
    public S3DGeometryMeshBuffer[] Buffers { get; set; }

  }

  public struct S3DGeometryMeshBuffer
  {
    public uint BufferId;
    public uint SubBufferOffset;
  }

  [Flags]
  public enum S3DMeshFlags : ulong
  {
    Unk_01 = 1 << 0x01,
    Unk_02 = 1 << 0x02,
    Unk_03 = 1 << 0x03,
    Unk_04 = 1 << 0x04,
    Unk_05 = 1 << 0x05,
    Unk_06 = 1 << 0x06,
    Tex0_4D = 1 << 0x07,
    Tex1_4D = 1 << 0x08,
    Tex2_4D = 1 << 0x09,
    Tex3_4D = 1 << 0x0A,
    Tex4_4D = 1 << 0x0B,
    Tex5_4D = 1 << 0x0C,
    Unk_0D = 1 << 0x0D,
    Norm_In_Vert4 = 1 << 0x0E,
    Tang0 = 1 << 0x0F,
    Tang1 = 1 << 0x10,
    Tang2 = 1 << 0x11,
    Tang3 = 1 << 0x12,
    Tang4 = 1 << 0x13,
    Unk_14 = 1 << 0x14,
    Unk_15 = 1 << 0x15,
    Unk_16 = 1 << 0x16,
    Unk_17 = 1 << 0x17,
    Unk_18 = 1 << 0x18,
    Compr_Vert = 1 << 0x19,
    Compr_Tex = 1 << 0x1A,
    Compr_Tang = 1 << 0x1B,
    Compr_Norm = 1 << 0x1C,
  }

}
