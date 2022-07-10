using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using H2AIndex.Common;
using H2AIndex.Models;
using H2AIndex.Processes;
using H2AIndex.Services;
using H2AIndex.UI.Modals;
using H2AIndex.Views;
using Microsoft.Extensions.DependencyInjection;
using PropertyChanged;

namespace H2AIndex.ViewModels
{

  [AddINotifyPropertyChangedInterface]
  public abstract class ViewModel : ObservableObject, IViewModel
  {

    #region Events

    public event EventHandler Disposing;

    #endregion

    #region Data Members

    private readonly IServiceProvider _serviceProvider;

    private bool _isDisposed;
    private bool _isInitialized;

    private ObservableCollection<IModal> _modalElements;

    #endregion

    #region Properties

    public bool IsBusy { get; protected set; }

    public ObservableCollection<IModal> Modals
    {
      get => _modalElements;
      private set => _modalElements = value;
    }

    protected IServiceProvider ServiceProvider
    {
      get => _serviceProvider;
    }

    public ICommand ShowExceptionModalCommand { get; }
    public ICommand ShowModalCommand { get; }
    public ICommand ShowViewModalCommand { get; }
    public ICommand ShowWebPageCommand { get; }

    #endregion

    #region Constructor

    protected ViewModel( IServiceProvider serviceProvider )
    {
      _serviceProvider = serviceProvider;
      _modalElements = new ObservableCollection<IModal>();

      ShowExceptionModalCommand = new Command<Exception>( exception => ShowExceptionModal( exception ) );
      ShowModalCommand = new Command<Type>( modalType => ShowModal( modalType ) );
      ShowViewModalCommand = new Command<Type>( viewType => ShowViewModal( viewType ) );
      ShowWebPageCommand = new Command<string>( OpenWebPage );
    }

    #endregion

    #region Public Methods

    public async Task Initialize()
    {
      if ( _isInitialized )
        return;

      await OnInitializing();

      _isInitialized = true;
    }

    #endregion

    #region Private Methods

    protected virtual Task OnInitializing()
      => Task.CompletedTask;

    #region File Methods

    protected Task<string> ShowFolderBrowserDialog(
      string title = null,
      string defaultPath = null )
    {
      var fileDialogService = ServiceProvider.GetRequiredService<IFileDialogService>();
      return fileDialogService.BrowseForDirectory( title, defaultPath );
    }

    protected Task<string[]> ShowOpenFileDialog(
      string title = null,
      string defaultFileName = null,
      string filter = null )
    {
      var fileDialogService = ServiceProvider.GetRequiredService<IFileDialogService>();
      return fileDialogService.BrowseForOpenFile( title, defaultFileName, filter );
    }

    protected Task<string> ShowSaveFileDialog(
      string title = null,
      string defaultFileName = null,
      string filter = null )
    {
      var fileDialogService = ServiceProvider.GetRequiredService<IFileDialogService>();
      return fileDialogService.BrowseForSaveFile( title, defaultFileName, filter );
    }

    #endregion

    protected void OpenWebPage( string url )
      => Process.Start( new ProcessStartInfo( url ) { UseShellExecute = true } );

    #region Modal Methods

    protected async Task RunProcess( IProcess process )
    {
      var modal = ServiceProvider.GetService<ProgressModal>();
      modal.DataContext = process;

      using ( modal )
      {
        Modals.Add( modal );
        modal.Show();
        IsBusy = true;

        await Task.Factory.StartNew( process.Execute, TaskCreationOptions.LongRunning );
        await process.CompletionTask;

        await modal.Hide();
        Modals.Remove( modal );
        IsBusy = false;
      }

      var statusList = process.StatusList;
      if ( statusList.HasErrors || statusList.HasWarnings )
        await ShowStatusListModal( statusList );
    }

    protected async Task<object> ShowModal( IModal modal )
    {
      using ( modal )
      {
        Modals.Add( modal );
        modal.Show();

        var result = await modal.Task;

        await modal.Hide();
        Modals.Remove( modal );

        return result;
      }
    }

    protected Task<object> ShowModal( Type modalType )
    {
      var modal = ( IModal ) ServiceProvider.GetService( modalType );
      return ShowModal( modal );
    }

    protected Task<object> ShowModal<TModal>()
      where TModal : Modal
      => ShowModal( typeof( TModal ) );

    protected Task<object> ShowExceptionModal( Exception exception )
    {
      var modal = new ExceptionModal( exception );
      return ShowModal( modal );
    }

    protected Task<object> ShowMessageModal( string title, string message, bool showOnMainView = false )
    {
      var modal = new MessageModal()
      {
        Title = title,
        Message = message
      };

      if ( showOnMainView )
      {
        var mainViewModel = ServiceProvider.GetRequiredService<MainViewModel>();
        return mainViewModel.ShowModal( modal );
      }
      else
        return ShowModal( modal );
    }

    protected Task<object> ShowStatusListModal( StatusList statusList )
    {
      var viewService = ServiceProvider.GetService<IViewService>();
      var view = viewService.GetViewWithDefaultViewModel( typeof( StatusListView ) );
      ( ( StatusListViewModel ) view.DataContext ).StatusList = statusList;

      var modal = new ViewHostWindowModal( view );
      modal.DataContext = view.DataContext;

      return ShowModal( modal );
    }

    protected Task<object> ShowViewModal( Type viewType, Action<object> configureViewModel = null )
    {
      var viewService = ServiceProvider.GetService<IViewService>();
      var view = viewService.GetViewWithDefaultViewModel( viewType );

      var modal = new ViewHostWindowModal( view );
      modal.DataContext = view.DataContext;

      if ( configureViewModel != null )
        configureViewModel( view.DataContext );

      return ShowModal( modal );
    }

    protected Task<object> ShowViewModal<TView>( Action<object> configureViewModel = null )
      where TView : IView
      => ShowViewModal( typeof( TView ), configureViewModel );

    protected ProgressViewModel ShowProgress()
    {
      var modal = ServiceProvider.GetService<ProgressModal>();
      var progress = modal.ViewModel;

      async void OnProgressDisposing( object sender, EventArgs e )
      {
        progress.Disposing -= OnProgressDisposing;

        await modal.Hide();
        IsBusy = false;
        Modals.Remove( modal );
      }

      progress.Disposing += OnProgressDisposing;

      Modals.Add( modal );
      modal.Show();
      IsBusy = true;

      return progress;
    }

    #endregion

    #region Preferences Methods

    protected PreferencesModel GetPreferences()
    {
      var prefService = ServiceProvider.GetRequiredService<IPreferencesService>();
      return prefService.Preferences;
    }

    protected Task SavePreferences()
    {
      var prefService = ServiceProvider.GetRequiredService<IPreferencesService>();
      return prefService.SavePreferences();
    }

    #endregion

    #endregion

    #region Overrides

    protected override void OnDisposing()
    {
      Disposing?.Invoke( this, EventArgs.Empty );
      base.OnDisposing();
    }

    #endregion

  }

}
