using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Index.Common;

namespace Index.Controls
{

  public partial class LoadingControl : UserControl
  {

    public LoadingControl()
    {
      InitializeComponent();
    }

    public Task Show()
    {
      Opacity = 0;
      Visibility = Visibility.Visible;

      var anim = new DoubleAnimation
      {
        From = 0.0,
        To = 1.0,
        FillBehavior = FillBehavior.HoldEnd,
        Duration = TimeSpan.FromSeconds( 0.10 )
      };

      var storyboard = new Storyboard();
      storyboard.Children.Add( anim );

      Storyboard.SetTarget( anim, this );
      Storyboard.SetTargetProperty( anim, new PropertyPath( OpacityProperty ) );

      return storyboard.BeginAsync();
    }

    public Task Hide()
    {
      var anim = new DoubleAnimation
      {
        From = 1.0,
        To = 0.0,
        FillBehavior = FillBehavior.Stop,
        Duration = TimeSpan.FromSeconds( 0.10 )
      };

      var storyboard = new Storyboard();
      storyboard.Children.Add( anim );
      storyboard.Completed += ( s, e ) => this.Visibility = Visibility.Hidden;

      Storyboard.SetTarget( anim, this );
      Storyboard.SetTargetProperty( anim, new PropertyPath( OpacityProperty ) );

      return storyboard.BeginAsync();
    }

  }

}
