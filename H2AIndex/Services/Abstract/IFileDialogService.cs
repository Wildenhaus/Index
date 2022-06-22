using System.Threading.Tasks;

namespace H2AIndex.Services
{

  public interface IFileDialogService
  {

    #region Public Methods

    Task<string> BrowseForDirectory(
      string title = null,
      string defaultPath = null );

    Task<string[]> BrowseForOpenFile(
      string title = null,
      string defaultFileName = null,
      string initialDirectory = null,
      string filter = null,
      bool multiselect = true );

    Task<string> BrowseForSaveFile(
      string title = null,
      string defaultFileName = null,
      string initialDirectory = null,
      string filter = null );

    #endregion

  }

}
