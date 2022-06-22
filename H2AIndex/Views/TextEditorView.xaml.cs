using System.Windows;
using System.Windows.Media;
using H2AIndex.ViewModels;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Search;

namespace H2AIndex.Views
{

  public partial class TextEditorView : View<TextEditorViewModel>
  {

    #region Constructor

    public TextEditorView()
    {
      InitializeComponent();
      SearchPanel.Install( Editor.TextArea );

      Editor.TextArea.TextView.BackgroundRenderers.Add(
        new HilightCurrentLineBackgroundRenderer( Editor ) );

    }

    #endregion

    #region Private Methods



    #endregion

    #region Embedded Types

    class HilightCurrentLineBackgroundRenderer : IBackgroundRenderer
    {

      #region Data Members

      private TextEditor _editor;
      private Brush _lineBackgroundBrush;
      private Brush _lineBorderBrush;
      private Pen _borderPen;

      #endregion

      #region Properties

      public KnownLayer Layer => KnownLayer.Background;

      #endregion

      #region Constructor

      public HilightCurrentLineBackgroundRenderer( TextEditor editor )
      {
        _editor = editor;
        _lineBackgroundBrush = ( Brush ) App.Current.FindResource( "Brush.Deep.1.Background.Static" );
        _lineBorderBrush = ( Brush ) App.Current.FindResource( "Brush.Deep.3.Border.Static" );
        _borderPen = new Pen( _lineBorderBrush, 1 );
      }

      #endregion

      #region Public Methods

      public void Draw( TextView textView, DrawingContext drawingContext )
      {
        if ( _editor.Document is null )
          return;

        textView.EnsureVisualLines();
        var currentLine = _editor.Document.GetLineByOffset( _editor.CaretOffset );
        foreach ( var rect in BackgroundGeometryBuilder.GetRectsForSegment( textView, currentLine ) )
        {
          // Draw Border
          var borderRect = new Rect( rect.X - 12, rect.Y, textView.ActualWidth + 14, rect.Height );
          drawingContext.DrawRectangle( _lineBackgroundBrush, _borderPen, borderRect );
        }
      }

      #endregion

    }

    #endregion

  }

}
