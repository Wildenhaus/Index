using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using H2AIndex.Common;
using H2AIndex.ViewModels;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.Wpf.SharpDX;
using Camera = HelixToolkit.Wpf.SharpDX.Camera;

namespace H2AIndex.Controls
{

  public partial class ModelViewerControl : UserControl, IDisposable
  {

    #region Data Members

    #region Dependency Properties

    public static readonly DependencyProperty CameraProperty = DependencyProperty.Register(
      nameof( Camera ),
      typeof( Camera ),
      typeof( ModelViewerControl ) );

    public static readonly DependencyProperty EffectsManagerProperty = DependencyProperty.Register(
      nameof( EffectsManager ),
      typeof( EffectsManager ),
      typeof( ModelViewerControl ) );

    public static readonly DependencyProperty ModelProperty = DependencyProperty.Register(
      nameof( Model ),
      typeof( SceneNodeGroupModel3D ),
      typeof( ModelViewerControl ) );

    public static readonly DependencyProperty MinMoveSpeedProperty = DependencyProperty.Register(
      nameof( MinMoveSpeed ),
      typeof( double ),
      typeof( ModelViewerControl ),
      new PropertyMetadata( 0.0001d, OnSpeedPropertyChanged ) );

    public static readonly DependencyProperty MoveSpeedProperty = DependencyProperty.Register(
      nameof( MoveSpeed ),
      typeof( double ),
      typeof( ModelViewerControl ),
      new PropertyMetadata( 0.001d, OnSpeedPropertyChanged ) );

    public static readonly DependencyProperty MaxMoveSpeedProperty = DependencyProperty.Register(
      nameof( MaxMoveSpeed ),
      typeof( double ),
      typeof( ModelViewerControl ),
      new PropertyMetadata( 1.0d, OnSpeedPropertyChanged ) );

    public static readonly DependencyProperty UseFlycamProperty = DependencyProperty.Register(
      nameof( UseFlycam ),
      typeof( bool ),
      typeof( ModelViewerControl ) );

    #endregion

    private bool _isMouseCaptured;
    private Point _lastMousePos;

    private double _moveSpeed;
    private double _minMoveSpeed;
    private double _maxMoveSpeed;

    private double _newMoveSpeed;
    private ActionThrottler _moveSpeedThrottler;

    private bool _isRendered;
    private ManualResetEvent _isFocusedEvent;
    private Thread _monitorThread;
    private CancellationTokenSource _monitorCts;

    #endregion

    #region Properties

    public Camera Camera
    {
      get => ( Camera ) GetValue( CameraProperty );
      set => SetValue( CameraProperty, value );
    }

    public EffectsManager EffectsManager
    {
      get => ( EffectsManager ) GetValue( EffectsManagerProperty );
      set => SetValue( EffectsManagerProperty, value );
    }

    public SceneNodeGroupModel3D Model
    {
      get => ( SceneNodeGroupModel3D ) GetValue( ModelProperty );
      set => SetValue( ModelProperty, value );
    }

    public double MinMoveSpeed
    {
      get => ( double ) GetValue( MinMoveSpeedProperty );
      set => SetValue( MinMoveSpeedProperty, value );
    }

    public double MoveSpeed
    {
      get => ( double ) GetValue( MoveSpeedProperty );
      set => SetValue( MoveSpeedProperty, value );
    }

    public double MaxMoveSpeed
    {
      get => ( double ) GetValue( MaxMoveSpeedProperty );
      set => SetValue( MaxMoveSpeedProperty, value );
    }

    public bool UseFlycam
    {
      get => ( bool ) GetValue( UseFlycamProperty );
      set => SetValue( UseFlycamProperty, value );
    }

    public Viewport3DX Viewport
    {
      get;
    }

    #endregion

    #region Constructor

    public ModelViewerControl()
    {
      InitializeComponent();

      Viewport = ViewportControl;
      RemoveDirectionalViewKeyBindings();

      InitializeMonitorThread();
      ViewportControl.PreviewKeyDown += OnViewportPreviewKeyDown;
      Viewport.OnRendered += OnRendered;
      DataContextChanged += OnDataContextChanged;

      _isRendered = true;
    }

    #endregion

    #region IDisposable Methods

    public void Dispose()
    {
      DataContextChanged -= OnDataContextChanged;
      ViewportControl.PreviewKeyDown -= OnViewportPreviewKeyDown;
      Viewport.OnRendered -= OnRendered;
      DisposeMonitorThread();
    }

    #endregion

    #region Overrides

    protected override void OnGotMouseCapture( MouseEventArgs e )
    {
      if ( UseFlycam )
        _isFocusedEvent.Set();
    }

    protected override void OnLostMouseCapture( MouseEventArgs e )
    {
      if ( UseFlycam )
        _isFocusedEvent.Reset();
    }

    protected override void OnPreviewMouseLeftButtonDown( MouseButtonEventArgs e )
    {
      base.OnPreviewMouseLeftButtonDown( e );

      if ( !UseFlycam )
        return;

      Focus();
      CaptureMouse();
      Cursor = Cursors.None;

      var capturePoint = PointToScreen( e.GetPosition( this ) );
      _lastMousePos = new Point( ( int ) capturePoint.X, ( int ) capturePoint.Y );
    }

    protected override void OnPreviewMouseLeftButtonUp( MouseButtonEventArgs e )
    {
      base.OnPreviewMouseLeftButtonUp( e );

      if ( !UseFlycam )
        return;

      ReleaseMouseCapture();
      Cursor = Cursors.Arrow;
    }

    #endregion

    #region Private Methods

    private void RemoveDirectionalViewKeyBindings()
    {
      var toRemove = new List<KeyBinding>();
      foreach ( var binding in Viewport.InputBindings )
        if ( binding is KeyBinding keyBinding )
          toRemove.Add( keyBinding );

      foreach ( var binding in toRemove )
        Viewport.InputBindings.Remove( binding );
    }

    private void InitializeMonitorThread()
    {
      _moveSpeedThrottler = new ActionThrottler( () =>
      {
        Dispatcher.Invoke( () => { MoveSpeed = _moveSpeed = _newMoveSpeed; }, DispatcherPriority.Send );
      }, 100 );

      _isFocusedEvent = new ManualResetEvent( false );
      _monitorCts = new CancellationTokenSource();
      _monitorThread = new Thread( () => MonitorThread( _monitorCts.Token ) );

      _monitorThread.Start();
    }

    private void DisposeMonitorThread()
    {
      _monitorCts.Cancel();
      _monitorThread.Join();

      _monitorCts.Dispose();
      _isFocusedEvent.Dispose();
    }

    private void MonitorThread( CancellationToken cancellationToken )
    {
      var waitHandles = new WaitHandle[]
      {
        cancellationToken.WaitHandle,
        _isFocusedEvent
      };

      while ( true )
      {
        WaitHandle.WaitAny( waitHandles );
        if ( cancellationToken.IsCancellationRequested )
          return;

        HandleKeyboard();
        HandleMouseMove();
      }
    }

    private void HandleKeyboard()
    {
      if ( !_isRendered )
        return;

      var wPressed = Win32.IsKeyPressed( WinKeys.W );
      var aPressed = Win32.IsKeyPressed( WinKeys.A );
      var sPressed = Win32.IsKeyPressed( WinKeys.S );
      var dPressed = Win32.IsKeyPressed( WinKeys.D );
      var qPressed = Win32.IsKeyPressed( WinKeys.Q );
      var ePressed = Win32.IsKeyPressed( WinKeys.E );
      var rPressed = Win32.IsKeyPressed( WinKeys.R );
      var fPressed = Win32.IsKeyPressed( WinKeys.F );
      var shiftPressed = Win32.IsKeyPressed( WinKeys.Shift );

      if ( !( wPressed || aPressed || sPressed || dPressed || qPressed || ePressed || rPressed || fPressed ) )
        return;

      var delta = new Vector3D();

      var speed = _moveSpeed;
      if ( shiftPressed )
        speed *= 2;

      if ( wPressed )
        delta.Z += speed;
      if ( aPressed )
        delta.X -= speed;
      if ( sPressed )
        delta.Z -= speed;
      if ( dPressed )
        delta.X += speed;
      if ( qPressed )
        delta.Y -= speed;
      if ( ePressed )
        delta.Y += speed;

      if ( rPressed || fPressed )
      {
        if ( rPressed )
        {
          _newMoveSpeed = Math.Clamp( _moveSpeed * 2, _minMoveSpeed, _maxMoveSpeed );
          _moveSpeedThrottler.Execute();
        }
        if ( fPressed )
        {
          _newMoveSpeed = Math.Clamp( _moveSpeed / 2, _minMoveSpeed, _maxMoveSpeed );
          _moveSpeedThrottler.Execute();
        }
      }

      _isRendered = false;
      Dispatcher.Invoke( () => Camera.MoveCameraPosition( delta ), DispatcherPriority.Send );
    }

    private void HandleMouseMove()
    {
      bool isMouseCaptured = false;
      Dispatcher.Invoke( () => isMouseCaptured = IsMouseCaptured, DispatcherPriority.Send );
      if ( !isMouseCaptured )
        return;

      var lastPos = _lastMousePos;
      if ( !Win32.GetCursorPosition( out var mousePos ) )
        return;

      var dX = mousePos.X - lastPos.X;
      var dY = mousePos.Y - lastPos.Y;

      if ( dX == 0 && dY == 0 )
        return;

      Vector3D lookDir;
      Vector3D upDir = new Vector3D( 0, 1, 0 );
      Dispatcher.Invoke( () => { lookDir = Camera.LookDirection; }, DispatcherPriority.Send );
      lookDir.Normalize();

      var rightDir = Vector3D.CrossProduct( lookDir, upDir );
      rightDir.Normalize();

      var q1 = new Quaternion( -upDir, 0.5 * dX );
      var q2 = new Quaternion( -rightDir, 0.5 * dY );
      var q = q1 * q2;

      var m = new Matrix3D();
      m.Rotate( q );

      Win32.SetCursorPosition( lastPos );
      var newLookDir = m.Transform( lookDir );
      var newUpDir = m.Transform( upDir );
      Dispatcher.Invoke( () => { Camera.LookDirection = newLookDir; Camera.UpDirection = newUpDir; }, DispatcherPriority.Send );

    }

    #endregion

    #region Event Handlers

    private void OnDataContextChanged( object sender, DependencyPropertyChangedEventArgs e )
    {
      var viewModel = DataContext as ModelViewModel;
      if ( viewModel is null )
        return;

      viewModel.Viewport = ViewportControl;
    }

    private void OnViewportPreviewKeyDown( object sender, KeyEventArgs e )
    {
      switch ( e.Key )
      {
        case Key.W:
        case Key.A:
        case Key.S:
        case Key.D:
        case Key.Q:
        case Key.E:
        case Key.R:
        case Key.F:
          e.Handled = true;
          break;
      }
    }

    private static void OnSpeedPropertyChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
    {
      var control = d as ModelViewerControl;
      if ( control is null )
        return;

      control._minMoveSpeed = control.MinMoveSpeed;
      control._moveSpeed = control.MoveSpeed;
      control._maxMoveSpeed = control.MaxMoveSpeed;
    }

    private void OnRendered( object sender, EventArgs e )
      => _isRendered = true;

    #endregion

  }

}
