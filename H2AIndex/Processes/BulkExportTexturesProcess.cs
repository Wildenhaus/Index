using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using H2AIndex.Models;
using Saber3D.Files;

namespace H2AIndex.Processes
{

  public class BulkExportTexturesProcess : ProcessBase
  {

    private IEnumerable<IS3DFile> _targetFiles;
    private IList<ExportTextureProcess> _processes;
    private readonly TextureExportOptionsModel _options;

    public BulkExportTexturesProcess( TextureExportOptionsModel options )
    {
      _options = options;
    }

    public BulkExportTexturesProcess( IEnumerable<IS3DFile> files, TextureExportOptionsModel options )
      : this( options )
    {
      _targetFiles = files;
    }

    protected override async Task OnInitializing()
    {
      IsIndeterminate = true;
      Status = "Preparing Bulk Export";

      if ( _targetFiles == null )
        _targetFiles = GatherFiles();

      _processes = CreateProcesses( _targetFiles );
    }

    protected override async Task OnExecuting()
    {
      CompletedUnits = 0;
      TotalUnits = _processes.Count;
      UnitName = "Textures Exported";
      IsIndeterminate = false;

      foreach ( var process in _processes )
      {
        Status = $"Exporting {process.TextureName}";
        await process.Execute();

        StatusList.Merge( process.StatusList );
        CompletedUnits++;
      }
    }

    private IEnumerable<IS3DFile> GatherFiles()
    {
      string[] filters = null;
      if ( !string.IsNullOrWhiteSpace( _options.Filters ) )
        filters = _options.Filters.Split( new char[] { ';' } );

      return H2AFileContext.Global.GetFiles( ".pct" ).Where( file =>
      {
        if ( filters is null )
          return true;

        foreach ( var filter in filters )
          if ( file.Name.ToLower().Contains( filter.ToLower() ) )
            return true;

        return false;
      } );
    }

    private IList<ExportTextureProcess> CreateProcesses( IEnumerable<IS3DFile> files )
      => files.Select( file => new ExportTextureProcess( file, _options ) ).ToList();

  }

}
