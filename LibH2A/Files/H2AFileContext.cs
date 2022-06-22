using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Saber3D.Files
{

  public interface IH2AFileContext
  {

    #region Events

    event EventHandler<IS3DFile> FileAdded;
    event EventHandler<IS3DFile> FileRemoved;

    #endregion

    #region Properties

    IReadOnlyDictionary<string, IS3DFile> Files { get; }

    #endregion

    #region Public Methods

    bool AddFile( IS3DFile file );
    bool RemoveFile( IS3DFile file );

    IS3DFile GetFile( string fileName );
    TFile GetFile<TFile>( string fileName ) where TFile : class, IS3DFile;

    IEnumerable<IS3DFile> GetFiles( string searchPattern );
    IEnumerable<TFile> GetFiles<TFile>( string searchPattern ) where TFile : class, IS3DFile;
    IEnumerable<TFile> GetFiles<TFile>() where TFile : class, IS3DFile;

    bool OpenDirectory( string path );
    bool OpenFile( string filePath );

    #endregion

  }

  public class H2AFileContext : IH2AFileContext
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

    private readonly SemaphoreSlim _fileLock;
    private ConcurrentDictionary<string, IS3DFile> _files;

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
      _fileLock = new SemaphoreSlim( 1 );
      _files = new ConcurrentDictionary<string, IS3DFile>();
    }

    #endregion

    #region Public Methods

    public bool AddFile( IS3DFile file )
    {
      bool filesAdded = false;

      if ( !_files.ContainsKey( file.Name ) )
      {
        _files.TryAdd( file.Name, file );
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

      _files.TryGetValue( fileName, out var file );
      return file;
    }

    public TFile GetFile<TFile>( string fileName )
      where TFile : class, IS3DFile
      => GetFile( fileName ) as TFile;

    public IEnumerable<TFile> GetFiles<TFile>()
      where TFile : class, IS3DFile
      => _files.Values.OfType<TFile>();

    public IEnumerable<IS3DFile> GetFiles( string searchPattern )
    {
      searchPattern = searchPattern.ToLower();

      foreach ( var file in _files.Values )
        if ( file.Name.ToLower().Contains( searchPattern ) )
          yield return file;
    }

    public IEnumerable<TFile> GetFiles<TFile>( string searchPattern )
      where TFile : class, IS3DFile
      => GetFiles( searchPattern ).OfType<TFile>();

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

      try
      {
        stream.AcquireLock();
        var file = S3DFileFactory.CreateFile( fileName, stream, 0, stream.Length );
        if ( file is null )
          return false;

        return AddFile( file );
      }
      finally { stream.ReleaseLock(); }
    }

    public bool RemoveFile( IS3DFile file )
    {
      if ( _files.TryRemove( file.Name, out _ ) )
      {
        FileRemoved?.Invoke( this, file );
        return true;
      }

      return false;
    }

    #endregion

  }

}
