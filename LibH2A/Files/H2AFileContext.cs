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
      lock ( _files )
      {
        if ( !_files.ContainsKey( file.Name ) )
        {
          _files.Add( file.Name, file );
          return true;
        }

        return false;
      }
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
      var files = Directory.EnumerateFiles( path, "*.*", SearchOption.AllDirectories );
      foreach ( var file in files )
        filesAdded |= OpenFile( file );

      return filesAdded;
    }

    public bool OpenFile( string filePath )
    {
      switch ( Path.GetExtension( filePath ) )
      {
        case ".pck":
          return OpenPckFile( filePath );

        default:
          return false;
      }
    }

    public bool RemoveFile( IS3DFile file )
    {
      return _files.Remove( file.Name );
    }

    #endregion

    #region Private Methods

    private bool OpenPckFile( string filePath )
    {
      //try
      //{
      var fileName = Path.GetFileNameWithoutExtension( filePath );
      var pckFile = S3DPackFile.FromFile( filePath );

      AddFile( pckFile );

      foreach ( var entry in pckFile.Entries.Values )
        if ( !entry.IsReference )
          AddFile( entry );

      return true;
      //}
      //catch ( Exception ex )
      //{
      //  return false;
      //}
    }

    #endregion

  }

}
