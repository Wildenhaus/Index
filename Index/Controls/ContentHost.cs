using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace Index.Controls
{

  public class ContentHost : Panel
  {

    #region Constants

    private static readonly TimeSpan ANIM_DURATION = TimeSpan.FromMilliseconds( 100 );
    private const double BLACKOUT_OPACITY = 0.5;
    private const double BLUR_RADIUS = 5;

    #endregion

    #region Data Members

    private Rectangle _blackoutRect;
    private LinkedList<UIElement> _childStack;

    #endregion

    #region Constructor

    public ContentHost()
    {
      _blackoutRect = new Rectangle { Fill = Brushes.Black };
      _childStack = new LinkedList<UIElement>();
    }

    #endregion

    #region Public Methods

    public void Push( UIElement element )
    {
      if ( _childStack.Count == 0 )
        Dispatcher.Invoke( AddBlackout );

      _childStack.AddLast( element );

      Children.Add( element );
      Dispatcher.Invoke( () => AnimateFade( element, 0, 1 ) );
      Dispatcher.Invoke( SetBlurs );
    }

    public bool Pop()
    {
      if ( _childStack.Count == 0 )
        return false;

      var element = _childStack.Last.Value;
      return Remove( element );
    }

    public bool Remove( UIElement element )
    {
      var node = _childStack.Find( element );
      if ( node == null )
        return false;

      _childStack.Remove( node );
      Dispatcher.Invoke( () => AnimateFade( element, 1, 0, Children.Remove ) );

      if ( _childStack.Count == 0 )
        Dispatcher.Invoke( RemoveBlackout );

      Dispatcher.Invoke( SetBlurs );
      return true;
    }

    #endregion

    #region Overrides

    protected override Size ArrangeOverride( Size finalSize )
    {
      var rect = new Rect( finalSize );
      foreach ( UIElement child in InternalChildren )
        child.Arrange( rect );

      return finalSize;
    }

    protected override Size MeasureOverride( Size availableSize )
    {
      foreach ( UIElement child in InternalChildren )
        child.Measure( availableSize );

      return availableSize;
    }

    #endregion

    #region Private Methods

    private void AddBlackout()
    {
      Children.Add( _blackoutRect );
      AnimateFade( _blackoutRect, 0, BLACKOUT_OPACITY );
    }

    private void RemoveBlackout()
    {
      AnimateFade( _blackoutRect, BLACKOUT_OPACITY, 0, Children.Remove );
    }

    private void AnimateFade( UIElement element, double startValue, double endValue, Action<UIElement> onComplete = null )
    {
      element.Opacity = startValue;

      var storyboard = new Storyboard();

      if ( onComplete != null )
        storyboard.Completed += ( s, e ) => onComplete( element );

      var anim = new DoubleAnimation()
      {
        From = startValue,
        To = endValue,
        Duration = ANIM_DURATION,
        FillBehavior = FillBehavior.HoldEnd
      };

      storyboard.Children.Add( anim );

      Storyboard.SetTarget( anim, element );
      Storyboard.SetTargetProperty( anim, new PropertyPath( OpacityProperty ) );

      storyboard.Begin();
    }

    private void SetBlurs()
    {
      var storyboard = new Storyboard();

      if ( _childStack.Count == 0 )
      {
        foreach ( UIElement child in Children )
          storyboard.Children.Add( CreateBlurAnimation( child, 0 ) );
      }
      else
      {
        var topmostChild = _childStack.Last.Value;
        foreach ( UIElement child in Children )
        {
          if ( child == topmostChild )
            storyboard.Children.Add( CreateBlurAnimation( child, 0 ) );
          else
            storyboard.Children.Add( CreateBlurAnimation( child, BLUR_RADIUS ) );
        }

      }

      storyboard.Begin();
    }

    private DoubleAnimation CreateBlurAnimation( UIElement element, double endValue )
    {
      var blurEffect = element.Effect as BlurEffect;
      if ( blurEffect is null )
      {
        element.Effect = blurEffect = new BlurEffect
        {
          Radius = 0,
          KernelType = KernelType.Gaussian,
          RenderingBias = RenderingBias.Performance
        };
      }

      var anim = new DoubleAnimation()
      {
        From = blurEffect.Radius,
        To = endValue,
        Duration = ANIM_DURATION,
        FillBehavior = FillBehavior.HoldEnd
      };

      Storyboard.SetTarget( anim, element );
      Storyboard.SetTargetProperty( anim, new PropertyPath( "Effect.Radius" ) );

      return anim;
    }

    #endregion

  }

}
