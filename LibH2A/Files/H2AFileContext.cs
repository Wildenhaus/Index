using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Saber3D.Files
{

  public class H2AFileContext
  {

    #region Events

    public event EventHandler<IS3DFile> FileAdded;
    public event EventHandler<IS3DFile> FileRemoved;

    #endregion

    #region Data Members

    public static IReadOnlyCollection<string> SupportedFileExtensions
    {
      get => S3DFileFactory.SupportedFileExtensions;
    }

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
      lock ( _files )
        if ( !_files.ContainsKey( file.Name ) )
        {
          _files.Add( file.Name, file );
          file.SetFileContext( this );
          filesAdded = true;

          FileAdded?.Invoke( this, file );
        }

      foreach ( var childFile in file.Children )
        filesAdded |= AddFile( childFile );

      return filesAdded;
    }

    public IS3DFile GetFile( string fileName )
    {
      fileName = fileName.ToLower();

      lock ( _files )
      {
        _files.TryGetValue( fileName, out var file );
        return file;
      }
    }

    public IEnumerable<IS3DFile> GetFiles( string searchPattern )
    {
      searchPattern = searchPattern.ToLower();

      lock ( _files )
      {
        foreach ( var file in _files.Values )
          if ( file.Name.ToLower().Contains( searchPattern ) )
            yield return file;
      }
    }

    public bool OpenDirectory( string path )
    {
      var filesAdded = false;
      var files = Directory.EnumerateFiles( path, "*.pck", SearchOption.AllDirectories );
      //foreach ( var file in files )
      //  filesAdded |= OpenFile( file );
      Parallel.ForEach( files, file => OpenFile( file ) );

      return filesAdded;
    }

    public bool OpenFile( string filePath )
    {
      var fileName = Path.GetFileName( filePath );
      var fileExt = Path.GetExtension( fileName );

      H2AStream stream;
      if ( fileExt == ".pck" )
        stream = H2ADecompressionStream.FromFile( filePath );
      else
        stream = H2AExtractedFileStream.FromFile( filePath );

      var file = S3DFileFactory.CreateFile( fileName, stream, 0, stream.Length );
      if ( file is null )
        return false;

      return AddFile( file );
    }

    public bool RemoveFile( IS3DFile file )
    {
      if ( _files.Remove( file.Name ) )
      {
        FileRemoved?.Invoke( this, file );
        return true;
      }

      return false;
    }

    #endregion

  }

}
