using System.Windows.Media.Imaging;
using PropertyChanged;

namespace Index.Models
{

  [AddINotifyPropertyChangedInterface]
  public class TextureImageModel
  {

    public int Index { get; set; }
    public BitmapImage ImageData { get; set; }

  }

}
