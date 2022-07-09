using System.Threading.Tasks;
using System.Windows.Forms;

namespace H2AIndex.Services
{

  public class FileDialogService : IFileDialogService
  {

    public async Task<string> BrowseForDirectory(
      string title = null,
      string defaultPath = null )
    {
      using ( var dialog = new FolderBrowserDialog() )
      {
        if ( !string.IsNullOrWhiteSpace( title ) )
        {
          dialog.Description = title;
          dialog.UseDescriptionForTitle = true;
        }

        if ( !string.IsNullOrWhiteSpace( defaultPath ) )
          dialog.InitialDirectory = defaultPath;

        if ( dialog.ShowDialog() != DialogResult.OK )
          return null;

        return dialog.SelectedPath;
      }
    }

    public async Task<string[]> BrowseForOpenFile(
      string title = null,
      string defaultFileName = null,
      string filter = null,
      bool multiselect = true )
    {
      using ( var dialog = new OpenFileDialog() )
      {
        dialog.CheckPathExists = true;
        dialog.Multiselect = multiselect;

        if ( !string.IsNullOrWhiteSpace( title ) )
          dialog.Title = title;

        if ( !string.IsNullOrWhiteSpace( defaultFileName ) )
          dialog.FileName = defaultFileName;

        if ( !string.IsNullOrWhiteSpace( filter ) )
          dialog.Filter = filter;

        if ( dialog.ShowDialog() != DialogResult.OK )
          return null;

        return dialog.FileNames;
      }
    }

    public async Task<string> BrowseForSaveFile(
      string title = null,
      string defaultFileName = null,
      string filter = null )
    {
      using ( var dialog = new SaveFileDialog() )
      {
        dialog.CheckPathExists = true;
        dialog.OverwritePrompt = true;

        if ( !string.IsNullOrWhiteSpace( title ) )
          dialog.Title = title;

        if ( !string.IsNullOrWhiteSpace( defaultFileName ) )
          dialog.FileName = defaultFileName;

        if ( !string.IsNullOrWhiteSpace( filter ) )
          dialog.Filter = filter;

        if ( dialog.ShowDialog() != DialogResult.OK )
          return null;

        return dialog.FileName;
      }
    }

  }

}
