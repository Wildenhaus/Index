using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using H2AIndex.Services.Abstract;

namespace H2AIndex.Services
{

  public class MeshIdentifierService : IMeshIdentifierService
  {

    #region Data Members

    private readonly string[] _lodStrings = new string[]
    {
      "_lod1",
      "_lod2",
      "_lod3",
      ".lod1",
      ".lod2",
      ".lod3",
    };

    private readonly string[] _volumeStrings = new string[]
    {
      "occ",
      "oclud",
      "_o#",
      "refl",
      "rays",
      "shadowcaster",
    };

    #endregion

    #region Properties

    public Regex LodRegex { get; }
    public Regex VolumeRegex { get; }
    public Regex LodOrVolumeRegex { get; }

    #endregion

    #region Constructor

    public MeshIdentifierService()
    {
      LodRegex = BuildRegex( _lodStrings );
      VolumeRegex = BuildRegex( _volumeStrings );
      LodOrVolumeRegex = BuildRegex( _lodStrings.Concat( _volumeStrings ) );
    }

    #endregion

    #region Public Methods

    public bool IsLod( string name )
      => LodRegex.IsMatch( name );

    public bool IsVolume( string name )
      => VolumeRegex.IsMatch( name );

    public bool IsLodOrVolume( string name )
      => LodOrVolumeRegex.IsMatch( name );

    #endregion

    #region Private Methods

    private static Regex BuildRegex( IEnumerable<string> regexStrings )
    {
      var strList = string.Join( '|', regexStrings );
      var regexStr = "(?=(" + strList + "))";

      return new Regex( regexStr, RegexOptions.IgnoreCase );
    }

    #endregion

  }
}
