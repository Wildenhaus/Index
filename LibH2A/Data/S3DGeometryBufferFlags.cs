using System;

namespace Saber3D.Data
{

  [Flags]
  public enum S3DGeometryBufferFlags : ulong
  {
    #pragma warning disable format
    // @formatter:off — disable formatter after this line

    _FACE = 0, // TODO: Assuming no flags means it's a face

    _VERT             = 1ul << 0x00,
    _VERT_2D          = 1ul << 0x01,
    _VERT_4D          = 1ul << 0x02,
    _COMPRESSED_VERT  = 1ul << 0x03,
    _WEIGHT1          = 1ul << 0x04,
    _WEIGHT2          = 1ul << 0x05,
    _WEIGHT3          = 1ul << 0x06,
    _WEIGHT4          = 1ul << 0x07,
    _INDEX            = 1ul << 0x08,
    _NORM             = 1ul << 0x09,
    _COMPRESSED_NORM  = 1ul << 0x0A,
    _NORM_IN_VERT4    = 1ul << 0x0B,
    _TANG0            = 1ul << 0x0C,
    _TANG1            = 1ul << 0x0D,
    _TANG2            = 1ul << 0x0E,
    _TANG3            = 1ul << 0x0F,
    _TANG4            = 1ul << 0x10,
    _COMPRESSED_TANG  = 1ul << 0x11,
    _COL0             = 1ul << 0x12,
    _COL1             = 1ul << 0x13,
    _COL2             = 1ul << 0x14,
    _COL3             = 1ul << 0x15,
    _COL4             = 1ul << 0x16,
    _TEX0             = 1ul << 0x17,
    _TEX1             = 1ul << 0x18,
    _TEX2             = 1ul << 0x19,
    _TEX3             = 1ul << 0x1A,
    _TEX4             = 1ul << 0x1B,
    _TEX5             = 1ul << 0x1C,
    _COMPRESSED_TEX_0 = 1ul << 0x1D,
    _COMPRESSED_TEX_1 = 1ul << 0x1E,
    _COMPRESSED_TEX_2 = 1ul << 0x1F,
    _COMPRESSED_TEX_3 = 1ul << 0x20,
    _COMPRESSED_TEX_4 = 1ul << 0x21,
    _COMPRESSED_TEX_5 = 1ul << 0x22,
    _TEX0_4D          = 1ul << 0x23,
    _TEX1_4D          = 1ul << 0x24,
    _TEX2_4D          = 1ul << 0x25,
    _TEX3_4D          = 1ul << 0x26,
    _TEX4_4D          = 1ul << 0x27,
    _TEX5_4D          = 1ul << 0x28,
    _AP_XENON         = 1ul << 0x29,
    _AP_PC            = 1ul << 0x2A,
    _AP_PS3           = 1ul << 0x2B,
    _AP_ORBIS         = 1ul << 0x2C,
    _AP_DURANGO       = 1ul << 0x2D,
    _NV_HW            = 1ul << 0x2E,
    _API_DX10         = 1ul << 0x2F,
    _API_DX11         = 1ul << 0x30,
    Z_SAMPLING_DOT    = 1ul << 0x31,
    Z_SAMPLING_FLOAT  = 1ul << 0x32,
    Unk34             = 1ul << 0x33,
    Unk35             = 1ul << 0x34,
    Unk36             = 1ul << 0x35,
    Unk37             = 1ul << 0x36,
    Unk38             = 1ul << 0x37,
    Unk39             = 1ul << 0x38,
    Unk3A             = 1ul << 0x39,
    Unk3B             = 1ul << 0x3A,
    Unk3C             = 1ul << 0x3B,
    Unk3D             = 1ul << 0x3C,
    Unk3E             = 1ul << 0x3D,
    Unk3F             = 1ul << 0x3E,
  }

}
