using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using H2AIndex.Common;
using H2AIndex.ViewModels;

namespace H2AIndex.UI.Modals
{

  public abstract class WindowModal : Modal
  {

    #region Data Members

    public static readonly DependencyProperty FooterButtonsProperty = DependencyProperty.RegisterAttached(
      nameof( FooterButtons ),
      typeof( ObservableCollection<Button> ),
      typeof( WindowModal ),
      new PropertyMetadata() );

    public static readonly DependencyProperty ModalHeightProperty = DependencyProperty.Register(
      nameof( ModalHeight ),
      typeof( double ),
      typeof( WindowModal ),
      new PropertyMetadata( 300.0 ) );

    public static readonly DependencyProperty ModalWidthProperty = DependencyProperty.Register(
      nameof( ModalWidth ),
      typeof( double ),
      typeof( WindowModal ),
      new PropertyMetadata( 600.0 ) );

    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
      nameof( Title ),
      typeof( string ),
      typeof( WindowModal ),
      new PropertyMetadata() );

    #endregion

    #region Properties

    public ObservableCollection<Button> FooterButtons
    {
      get => ( ObservableCollection<Button> ) GetValue( FooterButtonsProperty );
      set => SetValue( FooterButtonsProperty, value );
    }

    public double ModalHeight
    {
      get => ( double ) GetValue( ModalHeightProperty );
      set => SetValue( ModalHeightProperty, value );
    }

    public double ModalWidth
    {
      get => ( double ) GetValue( ModalWidthProperty );
      set => SetValue( ModalWidthProperty, value );
    }

    public string Title
    {
      get => ( string ) GetValue( TitleProperty );
      set => SetValue( TitleProperty, value );
    }

    #endregion

    #region Constructor

    protected WindowModal()
    {
      SetResourceReference( TemplateProperty, "Template.WindowModal" );

      FooterButtons = new ObservableCollection<Button>();
      FooterButtons.CollectionChanged += OnFooterButtonCollectionChange;
    }

    #endregion

    #region Overrides

    protected override void OnVisualParentChanged( DependencyObject oldParent )
    {
      // Add a close button if no buttons are present on the modal.
      if ( FooterButtons.Count == 0 )
        FooterButtons.Add( new Button { Content = "Close" } );

      base.OnVisualParentChanged( oldParent );
    }

    protected override void OnDisposing()
    {
      FooterButtons.CollectionChanged -= OnFooterButtonCollectionChange;
      FooterButtons.Clear();

      base.OnDisposing();
    }

    #endregion

    #region Event Handlers

    protected virtual void OnFooterButtonCollectionChange( object sender, NotifyCollectionChangedEventArgs e )
    {
      foreach ( Button newButton in e.NewItems )
      {
        if ( newButton.Command != null )
          continue;

        newButton.Command = new Command( () => CloseModal( newButton.Content ) );
      }
    }

    #endregion

  }

  public abstract class BoundWindowModal<TViewModel> : WindowModal
    where TViewModel : IViewModel
  {

    #region Properties

    public TViewModel ViewModel { get; }

    #endregion

    #region Constructor

    public BoundWindowModal( TViewModel viewModel )
    {
      ViewModel = viewModel;
      DataContext = ViewModel;
    }

    #endregion

  }

}
