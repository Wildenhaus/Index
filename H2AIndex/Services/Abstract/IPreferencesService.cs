using System.Threading.Tasks;
using H2AIndex.Models;

namespace H2AIndex.Services
{

  public interface IPreferencesService
  {

    #region Properties

    PreferencesModel Preferences { get; }

    #endregion

    #region Public Methods

    Task Initialize();

    Task<PreferencesModel> LoadPreferences();
    Task SavePreferences();

    #endregion

  }

}
