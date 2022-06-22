using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using DirectXTexNet;
using H2AIndex.Common;
using PropertyChanged;
using Saber3D.Data.Textures;

namespace H2AIndex.Models
{

  public class TextureModel : ObservableObject
  {

    #region Data Members

    private S3DPicture _pict;
    private ScratchImage _ddsImage;
    private TexMetadata _metadata;

    #endregion

    #region Properties

    public string Name { get; }
    public ScratchImage DdsImage => _ddsImage;

    public int Width => _pict.Width;
    public int Height => _pict.Height;
    public int Depth => _pict.Depth;
    public int FaceCount => _pict.Faces;
    public int MipLevels => _pict.MipMapCount;
    public S3DPictureFormat Format => _pict.Format;

    public IReadOnlyList<TextureImage> Faces { get; set; }

    [DependsOn( nameof( Width ), nameof( Height ), nameof( Depth ) )]
    public string DimensionString => $"{Width} x {Height} x {Depth}";

    [OnChangedMethod( nameof( OnSelectedFaceChanged ) )]
    public TextureImage SelectedFace { get; set; }

    public TextureImageMip SelectedMip { get; set; }

    public bool IsCubeMap
    {
      get => FaceCount > 1;
    }

    #endregion

    #region Constructor

    private TextureModel( string name, S3DPicture pict, ScratchImage ddsImage, TexMetadata metadata )
    {
      _pict = pict;
      _ddsImage = ddsImage;
      _metadata = metadata;

      Name = name;
    }

    public static TextureModel Create( string name, S3DPicture pict, ScratchImage ddsImage, TexMetadata metadata )
    {
      var model = new TextureModel( name, pict, ddsImage, metadata );

      var images = new List<TextureImage>();

      for ( var i = 0; i < pict.Faces; i++ )
      {
        var image = new TextureImage { Index = i };
        var imageMips = new List<TextureImageMip>( pict.MipMapCount );

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
