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

  public class BulkExportModelsProcess : ProcessBase
  {

    #region Data Members

    private IH2AFileContext _fileContext;

    private IEnumerable<TplFile> _targetFiles;
    private IList<ExportModelProcess> _processes;

    private ModelExportOptionsModel _modelOptions;
    private TextureExportOptionsModel _textureOptions;

    #endregion

    #region Properties

    public override bool CanCancel => true;

    #endregion

    #region Constructor

    public BulkExportModelsProcess( ModelExportOptionsModel modelOptions, TextureExportOptionsModel textureOptions )
    {
      _modelOptions = modelOptions;
      _textureOptions = textureOptions;

      _fileContext = ServiceProvider.GetRequiredService<IH2AFileContext>();
    }

    #endregion

    #region Overrides

    protected override async Task OnInitializing()
    {
      IsIndeterminate = true;
      Status = "Preparing Bulk Export";

      if ( _targetFiles is null )
        _targetFiles = GatherFiles();

      _processes = CreateProcesses( _targetFiles );
    }

    protected override async Task OnExecuting()
    {
      Status = "Exporting Models";
      CompletedUnits = 0;
      TotalUnits = _processes.Count;
      UnitName = "Models Exported";
      IsIndeterminate = false;

      if ( IsCancellationRequested )
        return;

      var processLock = new object();
      var executionBlock = new ActionBlock<ExportModelProcess>( async process =>
      {
        if ( IsCancellationRequested )
          return;

        await process.Execute();

        lock ( processLock )
          CompletedUnits++;
      },
      new ExecutionDataflowBlockOptions
      {
        MaxDegreeOfParallelism = Math.Min( 1, Environment.ProcessorCount ),
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
      _targetFiles = null;
      _processes = null;
    }

    #endregion

    #region Private Methods

    private IEnumerable<TplFile> GatherFiles()
    {
      string[] filters = null;
      if ( !string.IsNullOrWhiteSpace( _modelOptions.Filters ) )
        filters = _modelOptions.Filters.Split( ';', System.StringSplitOptions.RemoveEmptyEntries );

      return _fileContext.GetFiles<TplFile>().Where( file =>
      {
        if ( filters is null )
          return true;

        foreach ( var filter in filters )
          if ( file.Name.ToLower().Contains( filter.ToLower() ) )
            return true;

        return false;
      } );
    }

    private IList<ExportModelProcess> CreateProcesses( IEnumerable<TplFile> files )
      => files.Select( file => new ExportModelProcess( file, _modelOptions, _textureOptions ) ).ToList();

    #endregion

  }

}
