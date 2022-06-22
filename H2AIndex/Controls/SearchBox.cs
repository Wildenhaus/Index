using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace H2AIndex.Controls
{

  public class SearchBox : TextBox
  {

    #region Data Members

    public static DependencyProperty HasTextProperty = DependencyProperty.Register(
      nameof( HasText ),
      typeof( bool ),
      typeof( SearchBox ) );

    public static DependencyProperty PlaceholderTextProperty = DependencyProperty.Register(
      nameof( PlaceholderText ),
      typeof( string ),
      typeof( SearchBox ),
      new PropertyMetadata( "Search" ) );

    public static DependencyProperty TextChangedCommandProperty = DependencyProperty.Register(
      nameof( TextChangedCommand ),
      typeof( ICommand ),
      typeof( SearchBox ) );

    #endregion

    #region Properties

    public bool HasText
    {
      get => ( bool ) GetValue( HasTextProperty );
      set => SetValue( HasTextProperty, value );
    }

    public string PlaceholderText
    {
      get => ( string ) GetValue( PlaceholderTextProperty );
      set => SetValue( PlaceholderTextProperty, value );
    }

    public ICommand TextChangedCommand
    {
      get => ( ICommand ) GetValue( TextChangedCommandProperty );
      set => SetValue( TextChangedCommandProperty, value );
    }

    #endregion

    #region Constructor

    static SearchBox()
    {
      DefaultStyleKeyProperty.OverrideMetadata(
        typeof( SearchBox ),
        new FrameworkPropertyMetadata( typeof( SearchBox ) ) );
    }

    #endregion

    #region Overrides

    protected override void OnTextChanged( TextChangedEventArgs e )
    {
      base.OnTextChanged( e );

      HasText = Text.Length != 0;
      TextChangedCommand?.Execute( Text );
    }

    #endregion

  }

}
