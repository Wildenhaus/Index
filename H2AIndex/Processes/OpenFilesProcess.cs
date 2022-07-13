using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Saber3D.Files;

namespace H2AIndex.Processes
{

  public class OpenFilesProcess : ProcessBase<IEnumerable<string>>
  {

    #region Data Members

    private H2AFileContext _fileContext;

    private IEnumerable<string> _inputPaths;
    private string[] _filePaths;

    private List<string> _filesLoaded;

    #endregion

    #region Properties

    public override IEnumerable<string> Result
    {
      get => _filesLoaded;
    }

    #endregion

    #region Constructor

    public OpenFilesProcess( H2AFileContext fileContext, IEnumerable<string> paths )
    {
      _fileContext = fileContext;
      _inputPaths = paths;

      _filesLoaded = new List<string>();
    }

    public OpenFilesProcess( H2AFileContext fileContext, string path )
      : this( fileContext, new string[] { path } )
    {
    }

    #endregion

    #region Overrides

    protected override async Task OnInitializing()
    {
      _filePaths = GetFilePaths().ToArray();
    }

    protected override async Task OnExecuting()
    {
      Status = _filePaths.Length > 1 ? "Opening Files" : "Opening File";
      UnitName = _filePaths.Length > 1 ? "files opened" : "file opened";
      TotalUnits = _filePaths.Length;
      IsIndeterminate = _filePaths.Length == 1;

      var objLock = new object();
      Parallel.ForEach( _filePaths, filePath =>
      {
        var fileName = Path.GetFileName( filePath );
        try
        {
          if ( !_fileContext.OpenFile( filePath ) )
            StatusList.AddWarning( fileName, "Failed to open file." );
          else
            _filesLoaded.Add( fileName );
        }
        catch ( Exception ex )
        {
          StatusList.AddError( fileName, ex );
        }
        finally
        {
          lock ( objLock )
            CompletedUnits++;
        }
      } );
    }

    #endregion

    #region Private Methods

    private IEnumerable<string> GetFilePaths()
    {
      var visitedSet = new HashSet<string>();
      var queue = new Queue<string>( _inputPaths );
      while ( queue.TryDequeue( out var currentPath ) )
      {
        if ( !visitedSet.Add( currentPath ) )
          continue;

        if ( !File.Exists( currentPath ) && !Directory.Exists( currentPath ) )
        {
          StatusList.AddWarning( currentPath, "Path does not exist. Skipping." );
          continue;
        }

        var attributes = File.GetAttributes( currentPath );
        if ( attributes.HasFlag( FileAttributes.Directory ) )
        {
          var directoryFiles = Directory.GetFiles( currentPath, "*.*", SearchOption.AllDirectories )
            .Where( IsFileExtensionRecognized );

          foreach ( var file in directoryFiles )
            queue.Enqueue( file );

          continue;
        }
        else
        {
          if ( !IsFileExtensionRecognized( currentPath ) )
          {
            StatusList.AddWarning( currentPath, "File extension is not recognized. Skipping." );
            continue;
          }

          yield return currentPath;
        }
      }
    }

    private static bool IsFileExtensionRecognized( string filePath )
    {
      var ext = Path.GetExtension( filePath );
      return H2AFileContext.SupportedFileExtensions.Contains( ext );
    }

    #endregion

  }

}
