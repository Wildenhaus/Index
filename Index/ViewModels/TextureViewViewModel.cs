using System;
using System.Collections.ObjectModel;
using Index.Models;
using Saber3D.Data.Textures;

namespace Index.ViewModels
{

  public class TextureViewViewModel : ViewModel
  {

    public TextureImageModel SelectedTexture { get; set; }

    public string Name { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int Depth { get; set; }
    public int Faces { get; set; }
    public int MipMapCount { get; set; }
    public ObservableCollection<TextureImageModel> Images { get; }

    public TextureViewViewModel()
    {
      Images = new ObservableCollection<TextureImageModel>();
    }

    internal void ApplyMetadata( S3DPicture pict )
    {
      Width = pict.Width;
      Height = pict.Height;
      Depth = pict.Depth;
      Faces = pict.Faces;
      MipMapCount = pict.MipMapCount;
    }

    protected override void OnDisposing()
    {
      foreach ( var image in Images )
        image.ImageData.StreamSource.Dispose();

      SelectedTexture = null;
      Images.Clear();

      GC.Collect();
    }

  }

}
