using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Index.Controls
{

  public partial class TextureThumbnailControl : UserControl
  {

    public static readonly DependencyProperty ImageProperty = DependencyProperty.Register(
      nameof( Image ),
      typeof( BitmapImage ),
      typeof( TextureThumbnailControl ),
      new PropertyMetadata() );

    public BitmapImage Image
    {
      get => ( BitmapImage ) GetValue( ImageProperty );
      set => SetValue( ImageProperty, value );
    }

    public TextureThumbnailControl()
    {
      InitializeComponent();
      DataContext = this;
    }

  }
}
