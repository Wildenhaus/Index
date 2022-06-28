using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Rendering;
using Index.ViewModels;
using Saber3D.Files;

namespace Index.Views
{

  public partial class TextEditorView : View
  {
    private IS3DFile _file;

    public TextEditorViewViewModel ViewModel { get; }

    public TextEditorView( IS3DFile file )
    {
      InitializeComponent();
      _file = file;

      ViewModel = new TextEditorViewViewModel();
      DataContext = ViewModel;
    }

    protected override async Task OnInitializing()
    {
      var stream = _file.GetStream();
      var mem = new MemoryStream();
      stream.CopyTo( mem );
      mem.Position = 0;
      editor.Load( mem );

      //var r = new HighlightCurrentLineBackgroundRenderer( editor );
      //editor.TextArea.TextView.BackgroundRenderers.Add( r );
    }

  }

  public class HighlightCurrentLineBackgroundRenderer : IBackgroundRenderer
  {
    private TextEditor _editor;

    public HighlightCurrentLineBackgroundRenderer( TextEditor editor )
    {
      _editor = editor;
    }

    public KnownLayer Layer
    {
      get { return KnownLayer.Selection; }
    }

    public void Draw( TextView textView, DrawingContext drawingContext )
    {
      if ( _editor.Document == null )
        return;

      textView.EnsureVisualLines();

      int offset = _editor.CaretOffset;
      var line = _editor.Document.GetLineByOffset( offset );

      foreach ( var rect in BackgroundGeometryBuilder.GetRectsForSegment( textView, line ) )
      {
        drawingContext.DrawRectangle( Brushes.Red, null, new Rect( rect.Location, new Size( rect.Width, rect.Height ) ) );
      }
    }
  }

}
