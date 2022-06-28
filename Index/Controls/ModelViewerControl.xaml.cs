using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using Index.Common;

namespace Index.Controls
{

  public partial class ModelViewerControl : UserControl, IDisposable
  {

    /* Code is largely based off of the model renderer from Reclaimer.
     * Credits to Gravemind2401
     */

    #region Constants

    private const double RAD_089 = 1.5706217940;
    private const double RAD_090 = 1.5707963268;
    private const double RAD_360 = 6.2831853072;

    #endregion

    #region Data Members

    private Point _lastPoint;
    private DispatcherTimer _timer;
    private double _speed = 0.01;

    private TickProfiler _fpsProfiler;
    private AutoResetEvent _frameEvent;

    #endregion

    #region Properties

    public static readonly DependencyProperty ModelProperty = DependencyProperty.Register(
      nameof( Model ),
      typeof( Model3D ),
      typeof( ModelViewerControl ),
      new PropertyMetadata() );

    public Model3D Model
    {
      get => ( Model3D ) GetValue( ModelProperty );
      set
      {
        SetValue( ModelProperty, value );
        ZoomToBounds( Model );
      }
    }

    public static readonly DependencyProperty CameraPositionProperty = DependencyProperty.Register(
      nameof( CameraPosition ),
      typeof( Point3D ),
      typeof( ModelViewerControl ),
      new PropertyMetadata( new Point3D( 0, 1, 0 ) ) );

    public Point3D CameraPosition
    {
      get => ( Point3D ) GetValue( CameraPositionProperty );
      set => SetValue( CameraPositionProperty, value );
    }

    public static readonly DependencyProperty CameraDirectionProperty = DependencyProperty.Register(
      nameof( CameraDirection ),
      typeof( Vector3D ),
      typeof( ModelViewerControl ),
      new PropertyMetadata( new Vector3D( 0, 1, 0 ) ) );

    public Vector3D CameraDirection
    {
      get => ( Vector3D ) GetValue( CameraDirectionProperty );
      set => SetValue( CameraDirectionProperty, value );
    }

    public static readonly DependencyProperty YawProperty = DependencyProperty.Register(
      nameof( Yaw ),
      typeof( double ),
      typeof( ModelViewerControl ),
      new PropertyMetadata() );

    public double Yaw
    {
      get => ( double ) GetValue( YawProperty );
      set => SetValue( YawProperty, value );
    }

    public double Pitch
    {
      get;
      set;
    }

    public static readonly DependencyProperty FieldOfViewProperty = DependencyProperty.Register(
      nameof( FieldOfView ),
      typeof( double ),
      typeof( ModelViewerControl ),
      new PropertyMetadata( 90.0 ) );

    public double FieldOfView
    {
      get => ( double ) GetValue( FieldOfViewProperty );
      set => SetValue( FieldOfViewProperty, value );
    }

    public static readonly DependencyProperty FramesPerSecondProperty = DependencyProperty.Register(
      nameof( FramesPerSecond ),
      typeof( double ),
      typeof( ModelViewerControl ),
      new PropertyMetadata( 0.0 ) );

    public double FramesPerSecond
    {
      get => ( double ) GetValue( FramesPerSecondProperty );
      set => SetValue( FramesPerSecondProperty, value );
    }

    #endregion

    public ModelViewerControl()
    {
      InitializeComponent();

      _frameEvent = new AutoResetEvent( true );
      _fpsProfiler = new TickProfiler( 60, frameTime => FramesPerSecond = 1000 / frameTime );

      const double MS_PER_FRAME_60FPS = 1000.0 / 60;
      _timer = new DispatcherTimer( DispatcherPriority.Send ) { Interval = TimeSpan.FromMilliseconds( MS_PER_FRAME_60FPS ) };
      _timer.Tick += OnFrameTick;
      _timer.Start();

      Viewport.LayoutUpdated += ( s, e ) => _fpsProfiler.Register();
    }

    private void OnFrameTick( object? sender, EventArgs e )
    {
      _frameEvent.WaitOne();

      User32.GetCursorPos( out var mousePos );
      UpdateCameraPosition();
      UpdateCameraDirection( new Point( mousePos.X, mousePos.Y ) );

      //_fpsProfiler.Register();
      _frameEvent.Set();
    }

    protected override void OnPreviewMouseLeftButtonDown( MouseButtonEventArgs e )
    {
      base.OnPreviewMouseLeftButtonDown( e );

      Focus();
      CaptureMouse();
      Cursor = Cursors.None;

      var capturePoint = PointToScreen( e.GetPosition( this ) );
      _lastPoint = new Point( ( int ) capturePoint.X, ( int ) capturePoint.Y );
    }

    protected override void OnPreviewMouseLeftButtonUp( MouseButtonEventArgs e )
    {
      ReleaseMouseCapture();
      Cursor = Cursors.Arrow;
    }

    protected override void OnPreviewMouseWheel( MouseWheelEventArgs e )
    {
      //if ( !IsMouseCaptured )
      //  FieldOfView -= e.Delta / 100.0;
      //else

      if ( e.Delta < 0 )
        _speed = Clamp( _speed / 2, 0.001, 10 );
      else
        _speed = Clamp( _speed * 2, 0.001, 10 );
    }

    private void UpdateCameraDirection( Point mousePos )
    {
      if ( !IsMouseCaptured )
        return;

      var lastPoint = _lastPoint;
      var deltaX = mousePos.X - lastPoint.X;
      var deltaY = mousePos.Y - lastPoint.Y;

      Yaw += deltaX * 0.01;
      Pitch -= deltaY * 0.01;

      Yaw %= RAD_360;
      Pitch = Clamp( Pitch, -RAD_089, RAD_089 );

      CameraDirection = new Vector3D( Math.Sin( Yaw ), Math.Cos( Yaw ), Math.Tan( Pitch ) );
      User32.SetCursorPos( lastPoint.X, lastPoint.Y );

      return;
    }

    private bool UpdateCameraPosition()
    {
      if ( !IsMouseOver )
        return false;

      var speed = _speed;
      var nextPos = CameraPosition;
      var cameraDir = CameraDirection;
      var lookDirection = new Vector3D( cameraDir.X, cameraDir.Y, cameraDir.Z );

      if ( User32.IsKeyPressed( Keys.W ) )
      {
        nextPos.X += lookDirection.X * speed;
        nextPos.Y += lookDirection.Y * speed;
        nextPos.Z += lookDirection.Z * speed;
      }
      if ( User32.IsKeyPressed( Keys.A ) )
      {
        nextPos.X -= Math.Sin( Yaw + RAD_090 ) * speed;
        nextPos.Y -= Math.Cos( Yaw + RAD_090 ) * speed;
      }
      if ( User32.IsKeyPressed( Keys.S ) )
      {
        nextPos.X -= lookDirection.X * speed;
        nextPos.Y -= lookDirection.Y * speed;
        nextPos.Z -= lookDirection.Z * speed;
      }
      if ( User32.IsKeyPressed( Keys.D ) )
      {
        nextPos.X += Math.Sin( Yaw + RAD_090 ) * speed;
        nextPos.Y += Math.Cos( Yaw + RAD_090 ) * speed;
      }
      if ( User32.IsKeyPressed( Keys.Q ) )
        nextPos.Z -= speed;
      if ( User32.IsKeyPressed( Keys.E ) )
        nextPos.Z += speed;

      CameraPosition = new Point3D( nextPos.X, nextPos.Y, nextPos.Z );

      return true;
    }

    public void ZoomToBounds( Model3D model )
    {
      var bounds = model.Bounds;

      var p = new Point3D(
          ( bounds.X + bounds.SizeX / 2 ) * 10,
          ( bounds.Y + bounds.SizeY / 2 ),
          ( bounds.Z + bounds.SizeZ / 2 ) );

      MoveCamera( p, new Vector3D( -1, 0, 0 ) );
    }

    private void MoveCamera( Point3D position, Vector3D direction )
    {
      CameraPosition = position;
      CameraDirection = direction;
      NormalizeSet();
    }

    private void NormalizeSet()
    {
      var len = CameraDirection.Length;
      CameraDirection = new Vector3D( CameraDirection.X / len, CameraDirection.Y / len, CameraDirection.Z / len );
      Yaw = Math.Atan2( CameraDirection.X, CameraDirection.Z );
      Pitch = Math.Atan( CameraDirection.Y );
    }

    private static double Clamp( double val, double min, double max )
      => Math.Min( Math.Max( min, val ), max );

    public void Dispose()
    {
      _timer?.Stop();
    }
  }

}
