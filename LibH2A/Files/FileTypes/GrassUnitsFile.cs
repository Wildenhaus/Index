namespace Saber3D.Files.FileTypes
{

  [FileSignature( "1SERgrs_units" )]
  [FileExtension( ".grs_units" )]
  public class GrassUnitsFile : S3DFile
  {

    #region Properties

    public override string FileTypeDisplay => "Grass Units (.grs_units)";

    #endregion

    #region Constructor

    public GrassUnitsFile( string name, H2AStream baseStream,
      long dataStartOffset, long dataEndOffset,
      IS3DFile parent = null )
      : base( name, baseStream, dataStartOffset, dataEndOffset, parent )
    {
    }

    #endregion

  }

}
