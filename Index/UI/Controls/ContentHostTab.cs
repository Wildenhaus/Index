using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Index.Common;
using Index.Views;

namespace Index.UI.Controls
{

  public partial class ContentHostTab : TabItem
  {

    #region Data Members

    public static DependencyProperty CloseTabCommandProperty = DependencyProperty.Register(
      nameof( CloseTabCommand ),
      typeof( ICommand ),
      typeof( ContentHostTab ),
      new PropertyMetadata() );

    private ContentHost _contentHost;
    private View _view;

    #endregion

    #region Properties

    public ICommand CloseTabCommand
    {
      get => ( ICommand ) GetValue( CloseTabCommandProperty );
      set => SetValue( CloseTabCommandProperty, value );
    }

    public ContentHost ContentHost
    {
      get => _contentHost;
    }

    #endregion

    #region Constructor

    public ContentHostTab( string name, View view )
    {
      Header = name;
      _view = view;

      _contentHost = new ContentHost();
      _contentHost.Children.Add( _view );
      Content = _contentHost;

      CloseTabCommand = new DelegateCommand( _ => OnCloseTabClick() );

      SetResourceReference( TemplateProperty, "HostedTabItemTemplate" );
    }

    #endregion

    #region Event Handlers

    protected override void OnMouseDown( MouseButtonEventArgs e )
    {
      if ( e.ChangedButton == MouseButton.Middle && e.ButtonState == MouseButtonState.Pressed )
        OnCloseTabClick();
    }

    private void OnCloseTabClick()
    {
      var control = Parent as TabControl;
      if ( control is null )
        return;

      control.Items.Remove( this );
      _view.Dispose();
    }

    #endregion

  }

}
