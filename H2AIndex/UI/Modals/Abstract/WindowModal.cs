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

    public static readonly DependencyProperty ModalMaxHeightProperty = DependencyProperty.Register(
      nameof( ModalMaxHeight ),
      typeof( double ),
      typeof( WindowModal ),
      new PropertyMetadata( 500.0 ) );

    public static readonly DependencyProperty ModalMaxWidthProperty = DependencyProperty.Register(
      nameof( ModalMaxWidth ),
      typeof( double ),
      typeof( WindowModal ),
      new PropertyMetadata( 800.0 ) );

    public static readonly DependencyProperty ModalMinHeightProperty = DependencyProperty.Register(
      nameof( ModalMinHeight ),
      typeof( double ),
      typeof( WindowModal ),
      new PropertyMetadata( 300.0 ) );

    public static readonly DependencyProperty ModalMinWidthProperty = DependencyProperty.Register(
      nameof( ModalMinWidth ),
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

    public double ModalMinHeight
    {
      get => ( double ) GetValue( ModalMinHeightProperty );
      set => SetValue( ModalMinHeightProperty, value );
    }

    public double ModalMinWidth
    {
      get => ( double ) GetValue( ModalMinWidthProperty );
      set => SetValue( ModalMinWidthProperty, value );
    }

    public double ModalMaxHeight
    {
      get => ( double ) GetValue( ModalMaxHeightProperty );
      set => SetValue( ModalMaxHeightProperty, value );
    }

    public double ModalMaxWidth
    {
      get => ( double ) GetValue( ModalMaxWidthProperty );
      set => SetValue( ModalMaxWidthProperty, value );
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
      DataContextChanged += OnDataContextChanged;
    }

    #endregion

    #region Overrides

    protected override void OnVisualParentChanged( DependencyObject oldParent )
    {
      // Add a close button if no buttons are present on the modal.
      if ( FooterButtons.Count == 0 )
        OnAddDefaultFooterButtons();

      base.OnVisualParentChanged( oldParent );
    }

    protected virtual void OnAddDefaultFooterButtons()
    {
      FooterButtons.Add( new Button { Content = "Close" } );
    }

    protected override void OnDisposing()
    {
      DataContextChanged -= OnDataContextChanged;
      FooterButtons.CollectionChanged -= OnFooterButtonCollectionChange;
      FooterButtons.Clear();

      base.OnDisposing();
    }

    #endregion

    #region Event Handlers

    private void OnDataContextChanged( object sender, DependencyPropertyChangedEventArgs e )
    {
      if ( e.NewValue is IModalFooterButtons buttonProvider )
      {
        FooterButtons.Clear();
        foreach ( var button in buttonProvider.GetFooterButtons() )
          FooterButtons.Add( button );
      }
    }

    protected virtual void OnFooterButtonCollectionChange( object sender, NotifyCollectionChangedEventArgs e )
    {
      if ( e.NewItems is null )
        return;

      foreach ( Button newButton in e.NewItems )
      {
        if ( newButton.Command != null )
          continue;

        if ( newButton.CommandParameter != null )
          newButton.Command = new Command<object>( obj => CloseModal( obj ) );
        else
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
