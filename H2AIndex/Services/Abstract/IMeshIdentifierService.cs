using System.Text.RegularExpressions;

namespace H2AIndex.Services.Abstract
{

  public interface IMeshIdentifierService
  {

    #region Properties

    Regex LodRegex { get; }
    Regex VolumeRegex { get; }
    Regex LodOrVolumeRegex { get; }

    #endregion

    #region Public Methods

    bool IsLod( string name );
    bool IsVolume( string name );
    bool IsLodOrVolume( string name );

    #endregion

  }

}
