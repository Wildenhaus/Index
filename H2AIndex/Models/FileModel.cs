using Saber3D.Files;

namespace H2AIndex.Models
{

  public class FileModel
  {

    #region Data Members

    private readonly IS3DFile _file;

    #endregion

    #region Properties

    public string Name
    {
      get => _file.Name;
    }

    public string Extension
    {
      get => _file.Extension;
    }

    public string Group
    {
      get => _file.FileTypeDisplay;
    }

    public IS3DFile File
    {
      get => _file;
    }

    #endregion

    #region Constructor

    public FileModel( IS3DFile file )
    {
      _file = file;
    }

    #endregion

  }

}
