using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Index.Common;
using Index.UI.Controls;

namespace Index.Modals
{

  public abstract class WindowedModal : HostedModal
  {

    #region Properties

    public static readonly DependencyProperty ButtonsProperty = DependencyProperty.RegisterAttached(
      nameof( Buttons ),
      typeof( ObservableCollection<Button> ),
      typeof( WindowedModal ),
      new PropertyMetadata() );

    public ObservableCollection<Button> Buttons
    {
      get => ( ObservableCollection<Button> ) GetValue( ButtonsProperty );
      set => SetValue( ButtonsProperty, value );
    }

    public static readonly DependencyProperty ButtonClickCommandProperty = DependencyProperty.Register(
      nameof( ButtonClickCommand ),
      typeof( ICommand ),
      typeof( WindowedModal ),
      new FrameworkPropertyMetadata() );

    public ICommand ButtonClickCommand
    {
      get => ( ICommand ) GetValue( ButtonClickCommandProperty );
      set => SetValue( ButtonClickCommandProperty, value );
    }

    public static readonly DependencyProperty ShowCloseWindowButtonProperty = DependencyProperty.Register(
      nameof( ShowCloseWindowButton ),
      typeof( bool ),
      typeof( WindowedModal ),
      new FrameworkPropertyMetadata( false, FrameworkPropertyMetadataOptions.AffectsRender ) );

    public bool ShowCloseWindowButton
    {
      get => ( bool ) GetValue( ShowCloseWindowButtonProperty );
      set => SetValue( ShowCloseWindowButtonProperty, value );
    }

    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
      nameof( Title ),
      typeof( string ),
      typeof( WindowedModal ),
      new PropertyMetadata() );

    public string Title
    {
      get => ( string ) GetValue( TitleProperty );
      set => SetValue( TitleProperty, value );
    }

    public static readonly DependencyProperty MaxModalWidthProperty = DependencyProperty.Register(
      nameof( MaxModalWidth ),
      typeof( double ),
      typeof( WindowedModal ),
      new PropertyMetadata( 1000.0 ) );

    public double MaxModalWidth
    {
      get => ( double ) GetValue( MaxModalWidthProperty );
      set => SetValue( MaxModalWidthProperty, value );
    }

    public static readonly DependencyProperty WidthRatioProperty = DependencyProperty.Register(
      nameof( WidthRatio ),
      typeof( GridLength ),
      typeof( WindowedModal ),
      new PropertyMetadata( new GridLength( 1, GridUnitType.Star ) ) );

    public GridLength WidthRatio
    {
      get => ( GridLength ) GetValue( WidthRatioProperty );
      set => SetValue( WidthRatioProperty, value );
    }

    #endregion

    #region Constructor

    protected WindowedModal( ContentHost host )
      : base( host )
    {
      SetResourceReference( TemplateProperty, "WindowedModalTemplate" );
      ButtonClickCommand = new DelegateCommand( CloseModal );
      Buttons = new ObservableCollection<Button>();
      Buttons.CollectionChanged += Buttons_CollectionChanged;
    }

    private void Buttons_CollectionChanged( object? sender, NotifyCollectionChangedEventArgs e )
    {
      foreach ( Button button in e.NewItems )
      {
        if ( button.Command is null )
        {
          var binding = new Binding( nameof( ButtonClickCommand ) );
          binding.Source = this;

          button.SetBinding( Button.CommandProperty, binding );
        }
      }
    }

    protected WindowedModal( ContentHost host, IEnumerable<string> buttons )
      : this( host )
    {
      if ( buttons == null )
        buttons = new string[] { "Close" };

      foreach ( var buttonText in buttons )
      {
        var button = new Button { Content = buttonText };
        button.CommandParameter = button;

        Buttons.Add( button );
      }
    }

    #endregion

  }

}
