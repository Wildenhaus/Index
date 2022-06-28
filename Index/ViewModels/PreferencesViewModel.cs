using PropertyChanged;

namespace Index.ViewModels
{

  [AddINotifyPropertyChangedInterface]
  public class PreferencesViewModel : ViewModel
  {

    public static readonly PreferencesViewModel Defaults = new PreferencesViewModel
    {
      TexturePreviewQuality = 0.8f,
      TextureJpegExportQuality = 1.0f,
      TextureModelViewerQuality = 0.4f,

      ModelIncludeLods = false,
      ModelIncludeTriggerVolumes = false,
      ModelDefaultExportFormat = "fbx"
    };

    public string H2AGameDirectory { get; set; }
    public string ExportDirectory { get; set; }

    public float TexturePreviewQuality { get; set; }
    public float TextureJpegExportQuality { get; set; }
    public float TextureModelViewerQuality { get; set; }

    public bool ModelIncludeLods { get; set; }
    public bool ModelIncludeTriggerVolumes { get; set; }
    public string ModelDefaultExportFormat { get; set; }

  }

}
