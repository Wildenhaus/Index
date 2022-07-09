using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using DirectXTexNet;
using H2AIndex.Common;
using PropertyChanged;

namespace H2AIndex.Models
{

  public class TextureModel : ObservableObject
  {

    #region Data Members

    private ScratchImage _ddsImage;
    private TexMetadata _metadata;

    #endregion

    #region Properties

    public string Name { get; }
    public ScratchImage DdsImage => _ddsImage;

    public int Width => _metadata.Width;
    public int Height => _metadata.Height;
    public int Depth => _metadata.Depth;
    public int FaceCount => _metadata.ArraySize;
    public int MipLevels => _metadata.MipLevels;
    public DXGI_FORMAT Format => _metadata.Format;

    public IReadOnlyList<TextureImage> Faces { get; set; }

    [OnChangedMethod( nameof( OnSelectedFaceChanged ) )]
    public TextureImage SelectedFace { get; set; }

    public TextureImageMip SelectedMip { get; set; }

    public bool IsCubeMap
    {
      get => FaceCount > 1;
    }

    #endregion

    #region Constructor

    private TextureModel( string name, ScratchImage ddsImage, TexMetadata metadata )
    {
      _ddsImage = ddsImage;
      _metadata = metadata;

      Name = name;
    }

    public static TextureModel Create( string name, ScratchImage ddsImage, TexMetadata metadata )
    {
      var model = new TextureModel( name, ddsImage, metadata );

      var imageCount = ddsImage.GetImageCount() / metadata.MipLevels;
      var images = new List<TextureImage>( imageCount );

      for ( var i = 0; i < imageCount; i++ )
      {
        var image = new TextureImage { Index = i };
        var imageMips = new List<TextureImageMip>( metadata.MipLevels );

        for ( var j = 0; j < metadata.MipLevels; j++ )
          imageMips.Add( new TextureImageMip
          {
            MipLevel = j,
            ImageIndex = i * metadata.MipLevels + j
          } );

        image.MipMaps = imageMips;
        images.Add( image );
      }

      model.Faces = images;
      model.SelectedFace = model.Faces.FirstOrDefault();
      model.SelectedMip = model.SelectedFace?.MipMaps.FirstOrDefault();

      return model;
    }

    #endregion

    #region Overrides

    protected override void OnDisposing()
    {
      if ( Faces is null )
        return;

      foreach ( var image in Faces )
        image?.Dispose();

      _ddsImage?.Dispose();
      Faces = null;
    }

    #endregion

    #region Private Methods

    private void OnSelectedFaceChanged()
    {
      SelectedMip = SelectedFace?.MipMaps?.FirstOrDefault();
    }

    #endregion

  }

  public class TextureImage : ObservableObject
  {

    public int Index { get; set; }
    public IReadOnlyList<TextureImageMip> MipMaps { get; set; }

    public BitmapImage Preview
    {
      get => MipMaps?.FirstOrDefault()?.Preview;
    }

    protected override void OnDisposing()
    {
      foreach ( var mipmap in MipMaps )
        mipmap.Dispose();

      MipMaps = null;
    }

  }

  public class TextureImageMip : ObservableObject
  {
    public int ImageIndex { get; set; }
    public int MipLevel { get; set; }
    public BitmapImage Preview { get; set; }

    protected override void OnDisposing()
    {
      Preview?.StreamSource?.Dispose();
    }

  }

}
