using System.Collections.Generic;
using System.IO;

namespace Saber3D.Files
{

  public class H2AFileContext
  {

    #region Data Members

    public static H2AFileContext Global = new H2AFileContext();

    private Dictionary<string, IS3DFile> _files;

    #endregion

    #region Properties

    public IReadOnlyDictionary<string, IS3DFile> Files
    {
      get => _files;
    }

    #endregion

    #region Constructor

    public H2AFileContext()
    {
      _files = new Dictionary<string, IS3DFile>();
    }

    public static H2AFileContext FromDirectory( string path )
    {
      var context = new H2AFileContext();
      context.OpenDirectory( path );

      return context;
    }

    #endregion

    #region Public Methods

    public bool AddFile( IS3DFile file )
    {
      bool filesAdded = false;
      if ( !_files.ContainsKey( file.Name ) )
      {
        _files.Add( file.Name, file );
        file.SetFileContext( this );
        filesAdded = true;
      }

      foreach ( var childFile in file.Children )
        filesAdded |= AddFile( childFile );

      return filesAdded;
    }

    public IEnumerable<IS3DFile> GetFiles( string searchPattern )
    {
      searchPattern = searchPattern.ToLower();

      foreach ( var file in _files.Values )
        if ( file.Name.ToLower().Contains( searchPattern ) )
          yield return file;
    }

    public bool OpenDirectory( string path )
    {
      var filesAdded = false;
      var files = Directory.EnumerateFiles( path, "*.pck", SearchOption.AllDirectories );
      foreach ( var file in files )
        filesAdded |= OpenFile( file );

      return filesAdded;
    }

    public bool OpenFile( string filePath )
    {
      var fileName = Path.GetFileName( filePath );
      var fileExt = Path.GetExtension( fileName );

      var stream = H2ADecompressionStream.FromFile( filePath );
      var file = S3DFileFactory.CreateFile( fileName, stream );
      if ( file is null )
        return false;

      return AddFile( file );
    }

    public bool RemoveFile( IS3DFile file )
    {
      return _files.Remove( file.Name );
    }

    #endregion

  }

}
