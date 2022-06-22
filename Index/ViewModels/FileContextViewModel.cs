using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using DebounceThrottle;
using Saber3D.Files;

namespace Index.ViewModels
{

  public class FileContextViewModel : ViewModel
  {

    #region Data Members

    private H2AFileContext _context;
    private ObservableCollection<IS3DFile> _files;

    private readonly object _fileSynchronizer;
    private ThrottleDispatcher _fileAddThrottler;

    private ICollectionView _collectionViewSource;
    private string _searchTerm;

    #endregion

    #region Properties

    public ICollectionView Files
    {
      get => _collectionViewSource;
    }

    #endregion

    #region Constructor

    public FileContextViewModel()
    {
      _context = H2AFileContext.Global;
      _context.FileAdded += OnFileAdded;

      _files = new ObservableCollection<IS3DFile>();

      _collectionViewSource = CollectionViewSource.GetDefaultView( _files );
      _collectionViewSource.GroupDescriptions.Add( new PropertyGroupDescription( nameof( IS3DFile.FileTypeDisplay ) ) );
      _collectionViewSource.SortDescriptions.Add( new SortDescription( nameof( IS3DFile.Name ), ListSortDirection.Ascending ) );
      _collectionViewSource.Filter = ( obj ) =>
      {
        if ( string.IsNullOrWhiteSpace( _searchTerm ) )
          return true;

        var file = obj as IS3DFile;
        return file.Name.Contains( _searchTerm, System.StringComparison.InvariantCultureIgnoreCase );
      };

      _fileSynchronizer = new object();

      App.Current.Dispatcher.Invoke( () =>
      {
        BindingOperations.EnableCollectionSynchronization( _files, _fileSynchronizer );
      } );
    }

    #endregion

    #region Public Methods

    public void FilterFiles( string searchTerm )
    {
      _searchTerm = searchTerm;
      Refresh();
    }

    public void Refresh()
      => App.Current.Dispatcher.Invoke( () => _collectionViewSource.Refresh() );

    #endregion

    #region Event Handlers

    private void OnFileAdded( object? sender, IS3DFile e )
    {
      lock ( _fileSynchronizer )
        _files.Add( e );
    }

    #endregion

  }

}
