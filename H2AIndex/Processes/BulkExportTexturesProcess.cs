using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using H2AIndex.Models;
using Microsoft.Extensions.DependencyInjection;
using Saber3D.Files;
using Saber3D.Files.FileTypes;

namespace H2AIndex.Processes
{

  public class BulkExportTexturesProcess : ProcessBase
  {

    #region Data Members

    private IH2AFileContext _fileContext;

    private IEnumerable<PictureFile> _targetFiles;
    private IList<ExportTextureProcess> _processes;
    private readonly TextureExportOptionsModel _options;

    #endregion

    #region Properties

    public override bool CanCancel => true;

    #endregion

    #region Constructor

    public BulkExportTexturesProcess( TextureExportOptionsModel options )
    {
      _options = options;
      _fileContext = ServiceProvider.GetRequiredService<IH2AFileContext>();
    }

    public BulkExportTexturesProcess( IEnumerable<PictureFile> files, TextureExportOptionsModel options )
      : this( options )
    {
      _targetFiles = files;
    }

    #endregion

    #region Overrides

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
      Status = "Exporting Textures";
      CompletedUnits = 0;
      TotalUnits = _processes.Count;
      UnitName = "Textures Exported";
      IsIndeterminate = false;

      if ( IsCancellationRequested )
        return;

      var processLock = new object();

      var executionBlock = new ActionBlock<ExportTextureProcess>( async process =>
      {
        if ( IsCancellationRequested )
          return;

        await process.Execute();

        lock ( processLock )
          CompletedUnits++;
      },
      new ExecutionDataflowBlockOptions
      {
        MaxDegreeOfParallelism = Math.Min( 8, Environment.ProcessorCount ),
        EnsureOrdered = false
      } );

      foreach ( var process in _processes )
        executionBlock.Post( process );

      executionBlock.Complete();
      await executionBlock.Completion;

      foreach ( var process in _processes )
        StatusList.Merge( process.StatusList );
    }

    protected override async Task OnComplete()
    {
      _processes = null;
    }

    #endregion

    #region Private Methods

    private IEnumerable<PictureFile> GatherFiles()
    {
      string[] filters = null;
      if ( !string.IsNullOrWhiteSpace( _options.Filters ) )
        filters = _options.Filters.Split( new char[] { ';' } );

      return _fileContext.GetFiles<PictureFile>().Where( file =>
      {
        if ( filters is null )
          return true;

        foreach ( var filter in filters )
          if ( file.Name.ToLower().Contains( filter.ToLower() ) )
            return true;

        return false;
      } );
    }

    private IList<ExportTextureProcess> CreateProcesses( IEnumerable<PictureFile> files )
      => files.Select( file => new ExportTextureProcess( file, _options ) ).ToList();

    #endregion

  }

}
