using System;
using System.IO;
using Saber3D.Data;
using static Saber3D.Assertions;

namespace Saber3D.Serializers
{

  public class M3DSplineSerializer
  {

    public M3DSpline Deserialize( BinaryReader reader )
    {
      var splineData = ReadSplineProperties( reader );
      ValidateSplineData( splineData );

      return CreateSpline( splineData );
    }

    #region Private Methods

    private M3DSplineData ReadSplineProperties( BinaryReader reader )
    {
      var splineData = new M3DSplineData();

      while ( true )
      {
        var sentinel = ( PropertySentinel ) reader.ReadUInt16();
        var endOffset = reader.ReadUInt32();

        if ( ( ushort ) sentinel == 1 )
        {
          Assert( reader.BaseStream.Position == endOffset,
            "End of M3DSpline reached, but ending offset does not match." );
          break;
        }

        switch ( sentinel )
        {
          case PropertySentinel.SplineType:
            splineData.SplineType = ( SplineType ) reader.ReadByte();
            break;
          case PropertySentinel.Unk_01:
            splineData.Unk_01 = reader.ReadByte();
            break;
          case PropertySentinel.Unk_02:
            splineData.Unk_02 = reader.ReadByte();
            break;
          case PropertySentinel.Unk_03:
            splineData.Unk_03 = reader.ReadByte();
            break;
          case PropertySentinel.Count:
            splineData.Count = reader.ReadUInt32();
            break;
          case PropertySentinel.SizeInBytes:
            splineData.SizeInBytes = reader.ReadUInt32();
            break;
          case PropertySentinel.Data:
            splineData.Data = ReadSplineData( reader, splineData.SizeInBytes );
            break;
          default:
            continue;
            Fail( $"Unknown M3DSpline Property Sentinel: {( ushort ) sentinel:X}" );
            break;
        }

        Assert( reader.BaseStream.Position == endOffset,
          "Unexpected stream position after M3DSpline Property Read. " +
          $"Expected: 0x{endOffset:X}, Actual: 0x{reader.BaseStream.Position:X}" );
      }

      return splineData;
    }

    private float[] ReadSplineData( BinaryReader reader, uint sizeInBytes )
    {
      var elementCount = sizeInBytes / sizeof( float );

      var data = new float[ elementCount ];
      for ( var i = 0; i < elementCount; i++ )
        data[ i ] = reader.ReadSingle();

      return data;
    }

    private M3DSpline CreateSpline( M3DSplineData splineData )
    {
      M3DSpline spline;

      var splineType = splineData.SplineType;
      switch ( splineData.SplineType )
      {
        case SplineType.Linear1D:
          return new M3DSplineLinear1D( splineData );
        case SplineType.Linear2D:
          return new M3DSplineLinear2D( splineData );
        case SplineType.Linear3D:
          return new M3DSplineLinear3D( splineData );
        case SplineType.Hermit:
          return new M3DSplineHermit( splineData );
        case SplineType.Bezier2D:
          return new M3DSplineBezier2D( splineData );
        case SplineType.Bezier3D:
          return new M3DSplineBezier3D( splineData );
        case SplineType.Lagrange:
          return new M3DSplineLagrange( splineData );
        case SplineType.Quat:
          return new M3DSplineQuat( splineData );
        case SplineType.Color:
          return new M3DSplineQuat( splineData );
        default:
          throw new NotImplementedException( $"M3DSpline Type {splineType} is not implemented." );
      }
    }

    #endregion

    #region Validation Methods

    private void ValidateSplineData( M3DSplineData splineData )
    {
      // Assert valid SplineType
      var isSplineTypeDefined = Enum.IsDefined( typeof( SplineType ), splineData.SplineType );
      Assert( isSplineTypeDefined, $"Invalid M3DSpline Type: {splineData.SplineType:X}" );

      // Assert Valid Count
      Assert( splineData.Count > 0, $"Invalid M3DSpline Count: {splineData.Count}" );

      // Assert Valid SizeInBytes
      var count = splineData.Count;
      var sizeInBytes = splineData.SizeInBytes;
      Assert( sizeInBytes % count == 0, "Invalid M3DSpline SizeInBytes." );
    }

    #endregion

    #region Embedded Types

    private enum PropertySentinel : ushort
    {
      SplineType = 0xF0,
      Unk_01 = 0xF1,
      Unk_02 = 0xF2,
      Unk_03 = 0xF3,
      Count = 0xF4,
      SizeInBytes = 0xF5,
      Data = 0xF6
    }

    #endregion

  }

}
