using System;
using System.Threading.Tasks;
using H2AIndex.Common;
using H2AIndex.Models;
using H2AIndex.Services.Abstract;
using H2AIndex.ViewModels.Abstract;
using Microsoft.Extensions.DependencyInjection;
using Saber3D.Files;
using Saber3D.Files.FileTypes;

namespace H2AIndex.ViewModels
{

  [AcceptsFileType( typeof( PictureFile ) )]
  public class TextureViewModel : ViewModel, IDisposeWithView
  {

    #region Data Members

    private readonly ITextureConversionService _textureService;

    private readonly IS3DFile _file;

    #endregion

    #region Properties

    public TextureModel Texture { get; set; }

    #endregion

    #region Constructor

    public TextureViewModel( IServiceProvider serviceProvider, IS3DFile file )
      : base( serviceProvider )
    {
      _textureService = ServiceProvider.GetService<ITextureConversionService>();

      _file = file;
    }

    #endregion

    #region Overrides

    protected override async Task OnInitializing()
    {
      using ( var progress = ShowProgress() )
      {
        progress.IsIndeterminate = true;
        progress.Status = "Loading Texture";

        Texture = await _textureService.LoadTexture( _file );
      }
    }

    protected override void OnDisposing()
    {
      Texture?.Dispose();
      base.OnDisposing();

      GCHelper.ForceCollect();
    }

    #endregion

  }

}
