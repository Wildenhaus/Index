﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using H2AIndex.Common;
using H2AIndex.Services;
using Microsoft.Extensions.DependencyInjection;
using PropertyChanged;
using Saber3D.Files;

namespace H2AIndex.Models
{

  public class FileContextModel : ObservableObject
  {

    #region Data Members

    private readonly IH2AFileContext _context;
    private readonly ObservableCollection<FileModel> _files;

    private IReadOnlySet<string> _editorFileExtensions;

    private readonly object _collectionLock;
    private readonly ICollectionView _collectionViewSource;

    private ActionThrottler _throttler;
    private ConcurrentQueue<FileModel> _fileAddQueue;
    private ConcurrentQueue<FileModel> _fileRemoveQueue;
    private ConcurrentDictionary<string, FileModel> _fileLookup;

    private string _searchTerm;

    #endregion

    #region Properties

    public ICollectionView Files
    {
      get => _collectionViewSource;
    }

    [OnChangedMethod( nameof( RefreshFileTree ) )]
    public bool ShowUnsupportedFiles { get; set; }

    public ICommand SearchTermChangedCommand { get; }
    public ICommand OpenFileCommand { get; }
    public ICommand OpenDirectoryCommand { get; }

    #endregion

    #region Constructor

    public FileContextModel()
    {
      var serviceProvider = ( ( App ) App.Current ).ServiceProvider;

      // Initialize the underlying collection
      _collectionLock = new object();
      _files = new ObservableCollection<FileModel>();
      InitializeThreadSynchronization( _files, _collectionLock );

      // Initialize File Queues/Throttlers
      _throttler = new ActionThrottler( UpdateFiles, 1000 );
      _fileAddQueue = new ConcurrentQueue<FileModel>();
      _fileRemoveQueue = new ConcurrentQueue<FileModel>();
      _fileLookup = new ConcurrentDictionary<string, FileModel>();

      // Initialize File Extension Lookup
      var fileTypeService = serviceProvider.GetRequiredService<IFileTypeService>();
      _editorFileExtensions = fileTypeService.ExtensionsWithEditorSupport;

      // Initialize the LibH2A Context
      _context = serviceProvider.GetRequiredService<IH2AFileContext>();
      _context.FileAdded += OnFileAdded;
      _context.FileRemoved += OnFileRemoved;

      // Initialize the collection view source
      _collectionViewSource = InitializeCollectionView( _files );

      // Initialize Commands
      SearchTermChangedCommand = new Command<string>( OnSearchTermUpdated );

      AddExistingFiles();

      //_context.OpenFile( @"G:\h2a\re files\dervish__h.tpl" );
      //foreach ( var file in Directory.GetFiles( @"G:\h2a\d\", "*.pct", SearchOption.AllDirectories ) )
      //  if ( file.Contains( "masterchief" ) )
      //    _context.OpenFile( file );
    }

    #endregion

    #region Overrides

    protected override void OnDisposing()
    {
      _context.FileAdded -= OnFileRemoved;
      _context.FileRemoved -= OnFileAdded;
      _throttler.Dispose();
    }

    #endregion

    #region Private Methods

    private void InitializeThreadSynchronization(
      ObservableCollection<FileModel> files, object collectionLock )
    {
      // Initialize the underlying file collection with a lock so that we can
      // update the collection on other threads when the thread owns the lock.
      App.Current.Dispatcher.Invoke( () =>
      {
        BindingOperations.EnableCollectionSynchronization( files, collectionLock );
      } );
    }

    private ICollectionView InitializeCollectionView( ObservableCollection<FileModel> files )
    {
      var collectionView = CollectionViewSource.GetDefaultView( _files );
      collectionView.GroupDescriptions.Add( new PropertyGroupDescription( nameof( FileModel.Group ) ) );
      collectionView.SortDescriptions.Add( new SortDescription( nameof( FileModel.Group ), ListSortDirection.Ascending ) );
      collectionView.SortDescriptions.Add( new SortDescription( nameof( FileModel.Name ), ListSortDirection.Ascending ) );
      collectionView.Filter = OnFilterFiles;

      return collectionView;
    }

    private void UpdateFiles()
    {
      lock ( _collectionLock )
      {
        while ( _fileAddQueue.TryDequeue( out var fileToAdd ) )
          _files.Add( fileToAdd );

        while ( _fileRemoveQueue.TryDequeue( out var fileToRemove ) )
          _files.Remove( fileToRemove );
      }

      RefreshFileTree();
      if ( _fileAddQueue.Count > 0 || _fileRemoveQueue.Count > 0 )
        _throttler.Execute();
    }

    #endregion

    #region Event Handlers

    private void OnFileAdded( object sender, IS3DFile file )
    {
      // Explicitly exclude pck files from the FileTree
      if ( file.Extension == ".pck" )
        return;

      var model = new FileModel( file );
      if ( _fileLookup.TryAdd( file.Name, model ) )
      {
        _fileAddQueue.Enqueue( model );
        _throttler.Execute();
      }
    }

    private void OnFileRemoved( object sender, IS3DFile file )
    {
      if ( !_fileLookup.TryGetValue( file.Name, out var model ) )
        return;

      _fileRemoveQueue.Enqueue( model );
      _throttler.Execute();
    }

    private bool OnFilterFiles( object obj )
    {
      var file = obj as FileModel;
      if ( !ShowUnsupportedFiles && !_editorFileExtensions.Contains( file.Extension ) )
        return false;

      if ( !string.IsNullOrWhiteSpace( _searchTerm ) )
        return file.Name.Contains( _searchTerm, System.StringComparison.InvariantCultureIgnoreCase );

      return true;
    }

    private void OnSearchTermUpdated( string searchTerm )
    {
      _searchTerm = searchTerm;
      _throttler.Execute();
    }

    private void AddExistingFiles()
    {
      foreach ( var file in _context.Files.Values.ToArray() )
        OnFileAdded( this, file );
    }

    private void RefreshFileTree()
      => App.Current.Dispatcher.Invoke( _collectionViewSource.Refresh );

    #endregion

  }

}
