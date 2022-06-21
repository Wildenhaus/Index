using System;
using System.Collections.Generic;
using Saber3D.Data.Scripting;

namespace Saber3D.Data.Textures
{

  public class TextureDefinition
  {

    [ScriptingProperty( "__type_id" )]
    public String TypeId { get; set; }

    [ScriptingProperty( "convert_settings" )]
    public TextureDefinitionConvertSettings ConvertSettings { get; set; }

    [ScriptingProperty( "description" )]
    public String Description { get; set; }

    [ScriptingProperty( "game_material" )]
    public String GameMaterial { get; set; }

    [ScriptingProperty( "isUltraHiRes" )]
    public Boolean IsUltraHiRes { get; set; }

    [ScriptingProperty( "mapping" )]
    public TextureDefinitionMapping Mapping { get; set; }

    [ScriptingProperty( "materials" )]
    public Dictionary<string, TextureDefinitionMaterial> Materials { get; set; }

    [ScriptingProperty( "rendering" )]
    public TextureDefinitionRendering Rendering { get; set; }

    [ScriptingProperty( "shaders" )]
    public Dictionary<string, TextureDefinitionShader> Shaders { get; set; }

    [ScriptingProperty( "strm_no_lowres" )]
    public Boolean StreamDisableLowRes { get; set; }

    [ScriptingProperty( "strm_priority" )]
    public String StreamPriority { get; set; }

    [ScriptingProperty( "strm_priority_cine" )]
    public String StreamPriorityCinematic { get; set; }

    [ScriptingProperty( "tags" )]
    public String[] Tags { get; set; }

    [ScriptingProperty( "usage" )]
    public String Usage { get; set; }

    [ScriptingProperty( "version" )]
    public Single Version { get; set; }

  }

  public class TextureDefinitionMapping
  {

    [ScriptingProperty( "address_u" )]
    public String AddressU { get; set; }

    [ScriptingProperty( "address_v" )]
    public String AddressV { get; set; }

    [ScriptingProperty( "anisotropy" )]
    public Single Anisotropy { get; set; }

    [ScriptingProperty( "lod_bias" )]
    public Single LodBias { get; set; }

  }

  public class TextureDefinitionRendering
  {

    [ScriptingProperty( "akill_ref" )]
    public Int32 AlphaKillRef { get; set; }

    [ScriptingProperty( "decay" )]
    public TextureDefinitionDecay Decay { get; set; }

    [ScriptingProperty( "detail_density" )]
    public Single DetailDensity { get; set; }

    [ScriptingProperty( "detail_scale" )]
    public Single DetailScale { get; set; }

    [ScriptingProperty( "hdr_scale" )]
    public Single HdrScale { get; set; }

    [ScriptingProperty( "hm_range" )]
    public Single HeightMapRange { get; set; }

    [ScriptingProperty( "linear_rgb" )]
    public Boolean LinearRGB { get; set; }

    [ScriptingProperty( "sm_hi" )]
    public Boolean SmHi { get; set; }

    [ScriptingProperty( "tex_size_u" )]
    public Single TextureSizeU { get; set; }

    [ScriptingProperty( "tex_size_v" )]
    public Single TextureSizeV { get; set; }

    [ScriptingProperty( "use_akill" )]
    public Boolean UseAlphaKill { get; set; }

  }

  public class TextureDefinitionConvertSettings
  {

    [ScriptingProperty( "color" )]
    public S3DColor Color { get; set; }

    [ScriptingProperty( "fade_begin" )]
    public Int32 FadeBegin { get; set; }

    [ScriptingProperty( "fade_end" )]
    public Int32 FadeEnd { get; set; }

    [ScriptingProperty( "fade_flag" )]
    public Boolean FadeFlag { get; set; }

    [ScriptingProperty( "filter" )]
    public Int32 Filter { get; set; }

    [ScriptingProperty( "format_descr" )]
    public String FormatDescription { get; set; }

    [ScriptingProperty( "format_name" )]
    public String FormatName { get; set; }

    [ScriptingProperty( "fp16" )]
    public Boolean FP16 { get; set; }

    [ScriptingProperty( "left_handed" )]
    public Boolean LeftHanded { get; set; }

    [ScriptingProperty( "mipmap_level" )]
    public Int32 MipMapLevel { get; set; }

    [ScriptingProperty( "resample" )]
    public Int32 Resample { get; set; }

    [ScriptingProperty( "sharpen" )]
    public Int32 Sharpen { get; set; }

    [ScriptingProperty( "sharpness" )]
    public Boolean Sharpness { get; set; }

    [ScriptingProperty( "uncompressed_flag" )]
    public Boolean UncompressedFlag { get; set; }

  }

  public class TextureDefinitionMaterial
  {

    [ScriptingProperty( "__type_id" )]
    public String TypeId { get; set; }

    [ScriptingProperty( "default" )]
    public TextureDefinitionMaterial DefaultMaterial { get; set; }

    [ScriptingProperty( "game_material" )]
    public String GameMaterial { get; set; }

    [ScriptingProperty( "preset" )]
    public String Preset { get; set; }

    [ScriptingProperty( "shaders" )]
    public Dictionary<string, TextureDefinitionShader> Shaders { get; set; }

  }

  public class TextureDefinitionShader
  {

    [ScriptingProperty( "__type_id" )]
    public String TypeId { get; set; }

    [ScriptingProperty( "advanced" )]
    public TextureDefinititionShaderAdvanced Advanced { get; set; }

    [ScriptingProperty( "albedo_tint" )]
    public S3DColor AlbedoTint { get; set; }

    [ScriptingProperty( "alphaKill" )]
    public Boolean AlphaKillEnabled { get; set; }

    [ScriptingProperty( "alphaKillSmooth" )]
    public TextureDefinitionAlphaKillSmooth AlphaKillSmooth { get; set; }

    [ScriptingProperty( "ambient" )]
    public TextureDefinitionShaderAO AmbientOcclusion { get; set; }

    [ScriptingProperty( "animation" )]
    public TextureDefinitionShaderAnimation Animation { get; set; }

    [ScriptingProperty( "animationFrameBlend" )]
    public Boolean AnimationFrameBlendEnabled { get; set; }

    [ScriptingProperty( "animPeriod" )]
    public Single AnimationPeriod { get; set; }

    [ScriptingProperty( "anisotropic_spec" )]
    public TextureDefinitionShaderAnisoSpec AnisotropicSpec { get; set; }

    [ScriptingProperty( "autogenPatches" )]
    public Boolean AutogenPatches { get; set; }

    [ScriptingProperty( "blendMode" )]
    public String BlendMode { get; set; }

    [ScriptingProperty( "carpaint" )]
    public TextureDefinitionShaderCarPaint CarPaint { get; set; }

    [ScriptingProperty( "chameleon" )]
    public TextureDefinitionShaderChameleon Chameleon { get; set; }

    [ScriptingProperty( "colors" )]
    public TextureDefinitionShaderColors Colors { get; set; }

    [ScriptingProperty( "combiner" )]
    public String Combiner { get; set; }

    [ScriptingProperty( "depthPath" )]
    public Boolean DepthPath { get; set; }

    [ScriptingProperty( "detail" )]
    public TextureDefinitionShaderDetailMap Detail { get; set; }

    [ScriptingProperty( "detailDiffuse" )]
    public TextureDefinitionShaderDetailMap DetailDiffuse { get; set; }

    [ScriptingProperty( "diffuse" )]
    public TextureDefinitionShaderDiffuse Diffuse { get; set; }

    [ScriptingProperty( "disable_fog" )]
    public Boolean DisableFog { get; set; }

    [ScriptingProperty( "distort" )]
    public TextureDefinitionShaderDistortion Distort { get; set; }

    [ScriptingProperty( "distortBackground" )]
    public TextureDefinitionShaderDistortionBackground DistortBackground { get; set; }

    [ScriptingProperty( "doubleSideLighting" )]
    public TextureDefinitionShaderDoubleSideLighting DoubleSideLighting { get; set; }

    [ScriptingProperty( "emissive" )]
    public TextureDefinitionShaderEmissive emissive { get; set; }

    [ScriptingProperty( "eyes" )]
    public TextureDefinitionShaderEyes Eyes { get; set; }

    [ScriptingProperty( "fadeDist" )]
    public Single DadeDist { get; set; }

    [ScriptingProperty( "fakeLight" )]
    public TextureDefinitionShaderFakeLight FakeLight { get; set; }

    [ScriptingProperty( "foam" )]
    public TextureDefinitionShaderFoam Foam { get; set; }

    [ScriptingProperty( "fresnel_intensity" )]
    public Single FresnelIntensity { get; set; }

    [ScriptingProperty( "fresnel_power" )]
    public Single FresnelPower { get; set; }

    [ScriptingProperty( "glass" )]
    public TextureDefinitionShaderGlass Glass { get; set; }

    [ScriptingProperty( "glossiness" )]
    public TextureDefinitionShaderGlossiness Glossiness { get; set; }

    [ScriptingProperty( "gradientBlink" )]
    public TextureDefinitionShaderGradient GradientBlink { get; set; }

    [ScriptingProperty( "gradientScroll" )]
    public TextureDefinitionShaderGradient GradientScroll { get; set; }

    [ScriptingProperty( "layer1" )]
    public TextureDefinitionShaderLayer ShaderLayer1 { get; set; }

    [ScriptingProperty( "layer2" )]
    public TextureDefinitionShaderLayer ShaderLayer2 { get; set; }

    [ScriptingProperty( "layerBase" )]
    public TextureDefinitionShaderLayer ShaderLayerBase { get; set; }

    [ScriptingProperty( "layerSecond" )]
    public TextureDefinitionTextureLayer TextureLayerSecond { get; set; }

    [ScriptingProperty( "layerThird" )]
    public TextureDefinitionTextureLayer TextureLayerThird { get; set; }

    [ScriptingProperty( "maps" )]
    public TextureDefinitionShaderMaps maps { get; set; }

    [ScriptingProperty( "maskOffDistortion" )]
    public Boolean MaskOffDistortionEnabled { get; set; }

    [ScriptingProperty( "metalness" )]
    public S3DColor Metalness { get; set; }

    [ScriptingProperty( "nmScale" )]
    public Single NormalMapScale { get; set; }

    [ScriptingProperty( "no_backface_culling" )]
    public Boolean NoBackfaceCulling { get; set; }

    [ScriptingProperty( "no_ssao" )]
    public Boolean NoSsao { get; set; }

    [ScriptingProperty( "noCull" )]
    public Boolean NoCulling { get; set; }

    [ScriptingProperty( "noVertexColor" )]
    public Boolean NoVertexColor { get; set; }

    [ScriptingProperty( "overrideAffixScroll" )]
    public TextureDefinitionShaderOverrideAffixScroll OverrideAffixScroll { get; set; }

    [ScriptingProperty( "parallax" )]
    public TextureDefinitionShaderParallax Parallax { get; set; }

    [ScriptingProperty( "params" )]
    public TextureDefinitionShaderParams ShaderParams { get; set; }

    [ScriptingProperty( "particle" )]
    public Boolean ParticleEnabled { get; set; }

    [ScriptingProperty( "perturbation" )]
    public TextureDefinitionShaderPerturbation Perturbation { get; set; }

    [ScriptingProperty( "preset" )]
    public String Preset { get; set; }

    [ScriptingProperty( "reflection" )]
    public TextureDefinitionShaderReflection Reflection { get; set; }

    [ScriptingProperty( "refraction" )]
    public TextureDefinitionShaderRefraction Refraction { get; set; }

    [ScriptingProperty( "scale" )]
    public Single Scale { get; set; }

    [ScriptingProperty( "scrollSpeedScale" )]
    public Single ScrollSpeedScale { get; set; }

    [ScriptingProperty( "skin" )]
    public TextureDefinitionShaderSkin Skin { get; set; }

    [ScriptingProperty( "softFreshnel" )]
    public TextureDefinitionShaderSoftFresnel SoftFresnel { get; set; }

    [ScriptingProperty( "softZ" )]
    public TextureDefinitionShaderSoftZ SoftZ { get; set; }

    [ScriptingProperty( "specular" )]
    public TextureDefinitionShaderSpecular Specular { get; set; }

    [ScriptingProperty( "sprite" )]
    public Boolean Sprite { get; set; }

    [ScriptingProperty( "tex" )]
    public Dictionary<string, string> Textures { get; set; }

    [ScriptingProperty( "texNM" )]
    public String TextureNormalMap { get; set; }

    [ScriptingProperty( "tint" )]
    public S3DColor Tint { get; set; }

    [ScriptingProperty( "tintByMask" )]
    public TextureDefinitionShaderTintByMask TintByMask { get; set; }

    [ScriptingProperty( "translucency" )]
    public TextureDefinitionShaderTranslucency Translucency { get; set; }

    [ScriptingProperty( "useBillboards" )]
    public Boolean UseBillboards { get; set; }

    [ScriptingProperty( "useHDR" )]
    public Boolean UseHDR { get; set; }

    [ScriptingProperty( "writeDepth" )]
    public Boolean WriteDepth { get; set; }

    [ScriptingProperty( "z_func_less" )]
    public Boolean ZFuncLessEnabled { get; set; }

    [ScriptingProperty( "zTest" )]
    public Boolean ZTestEnabled { get; set; }

  }

  public class TextureDefinitionShaderGlossiness
  {

    [ScriptingProperty( "bias" )]
    public Single Bias { get; set; }

    [ScriptingProperty( "scale" )]
    public Single Scale { get; set; }

  }

  public class TextureDefinitionShaderDetailMap
  {

    [ScriptingProperty( "density" )]
    public Single Sensity { get; set; }

    [ScriptingProperty( "diffuse_tex" )]
    public String DiffuseTexture { get; set; }

    [ScriptingProperty( "scale" )]
    public Single Scale { get; set; }

    [ScriptingProperty( "tex" )]
    public String Texture { get; set; }

    [ScriptingProperty( "useDetailMask" )]
    public Boolean UseDetailMask { get; set; }

  }

  public class TextureDefinitionShaderEmissive
  {

    [ScriptingProperty( "adaptiveIntensity" )]
    public Boolean AdaptiveIntensity { get; set; }

    [ScriptingProperty( "animation" )]
    public TextureDefinitionShaderAnimation Animation { get; set; }

    [ScriptingProperty( "bloomIntensity" )]
    public Single BloomIntensity { get; set; }

    [ScriptingProperty( "intensity" )]
    public Single Intensity { get; set; }

    [ScriptingProperty( "tint" )]
    public S3DColor Tint { get; set; }

  }

  public class TextureDefinitionShaderSoftFresnel
  {

    [ScriptingProperty( "edge" )]
    public Boolean Edge { get; set; }

    [ScriptingProperty( "edgeHighlightIntensity" )]
    public Single EdgeHighlightIntensity { get; set; }

    [ScriptingProperty( "enabled" )]
    public Boolean Enabled { get; set; }

    [ScriptingProperty( "power" )]
    public Single Power { get; set; }

  }

  public class TextureDefinitionTextureLayer
  {

    [ScriptingProperty( "alphaSource" )]
    public String AlphaSource { get; set; }

    [ScriptingProperty( "blendMode" )]
    public String BlendMode { get; set; }

    [ScriptingProperty( "speedScale" )]
    public Single SpeedScale { get; set; }

    [ScriptingProperty( "texture" )]
    public String Texture { get; set; }

    [ScriptingProperty( "textureRotation" )]
    public Single TextureRotation { get; set; }

    [ScriptingProperty( "textureScaleX" )]
    public Single TextureScaleX { get; set; }

    [ScriptingProperty( "textureScaleY" )]
    public Single TextureScaleY { get; set; }

  }

  public class TextureDefinitionShaderDistortion
  {

    [ScriptingProperty( "distortionScale" )]
    public Single DistortionScale { get; set; }

    [ScriptingProperty( "distortTexture" )]
    public String DistortTexture { get; set; }

    [ScriptingProperty( "distortTextureRotation" )]
    public Single DistortTextureRotation { get; set; }

    [ScriptingProperty( "distortTextureScaleX" )]
    public Single DistortTextureScaleX { get; set; }

    [ScriptingProperty( "distortTextureScaleY" )]
    public Single DistortTextureScaleY { get; set; }

    [ScriptingProperty( "speedScale" )]
    public Single SpeedScale { get; set; }

    [ScriptingProperty( "useWCS" )]
    public Boolean UseWCS { get; set; }

  }

  public class TextureDefinitionShaderGradient
  {

    [ScriptingProperty( "speed" )]
    public Single Speed { get; set; }

    [ScriptingProperty( "tex" )]
    public String Texture { get; set; }

    [ScriptingProperty( "texPhaseOffset" )]
    public String TexturePhaseOffset { get; set; }

  }

  public class TextureDefinitionShaderSoftZ
  {

    [ScriptingProperty( "enabled" )]
    public Boolean Enabled { get; set; }

    [ScriptingProperty( "inverted" )]
    public Boolean Inverted { get; set; }

    [ScriptingProperty( "nearEnabled" )]
    public Boolean NearEnabled { get; set; }

    [ScriptingProperty( "nearRange" )]
    public Single NearRange { get; set; }

    [ScriptingProperty( "range" )]
    public Single Range { get; set; }

  }

  public class TextureDefinitionAlphaKillSmooth
  {

    [ScriptingProperty( "enable" )]
    public Boolean Enable { get; set; }

    [ScriptingProperty( "smoothness" )]
    public Single Smoothness { get; set; }

  }

  public class TextureDefinitionShaderDistortionBackground
  {

    [ScriptingProperty( "affectedBySelfDistortion" )]
    public Boolean AffectedBySelfDistortion { get; set; }

    [ScriptingProperty( "strength" )]
    public Single Strength { get; set; }

    [ScriptingProperty( "texture" )]
    public String Texture { get; set; }

    [ScriptingProperty( "textureRotation" )]
    public Single TextureRotation { get; set; }

    [ScriptingProperty( "textureScaleX" )]
    public Single TextureScaleX { get; set; }

    [ScriptingProperty( "textureScaleY" )]
    public Single TextureScaleY { get; set; }

    [ScriptingProperty( "textureSpeedScale" )]
    public Single TextureSpeedScale { get; set; }

  }

  public class TextureDefinitionShaderCarPaint
  {

    [ScriptingProperty( "glossiness" )]
    public TextureDefinitionShaderGlossiness Glossiness { get; set; }

    [ScriptingProperty( "metalness" )]
    public S3DColor Metalness { get; set; }

  }

  public class TextureDefinitionShaderChameleon
  {

    [ScriptingProperty( "color0" )]
    public S3DColor Color0 { get; set; }

    [ScriptingProperty( "color1" )]
    public S3DColor Color1 { get; set; }

    [ScriptingProperty( "color2" )]
    public S3DColor Color2 { get; set; }

    [ScriptingProperty( "color3" )]
    public S3DColor Color3 { get; set; }

    [ScriptingProperty( "enabled" )]
    public Boolean Enabled { get; set; }

    [ScriptingProperty( "fresnelPwr" )]
    public Single FresnelPwr { get; set; }

    [ScriptingProperty( "offset1" )]
    public Single Offset1 { get; set; }

    [ScriptingProperty( "offset2" )]
    public Single Offset2 { get; set; }

  }

  public class TextureDefinitionShaderAnisoSpec
  {

    [ScriptingProperty( "angle" )]
    public Single Angle { get; set; }

    [ScriptingProperty( "enabled" )]
    public Boolean Enabled { get; set; }

    [ScriptingProperty( "extra" )]
    public TextureDefinitionShaderAnisoSpecExtra Extra { get; set; }

    [ScriptingProperty( "intensity" )]
    public Single Intensity { get; set; }

    [ScriptingProperty( "tint" )]
    public S3DColor Tint { get; set; }

  }

  public class TextureDefinitionShaderAnisoSpecExtra
  {

    [ScriptingProperty( "intensity" )]
    public Single Intensity { get; set; }

    [ScriptingProperty( "shift" )]
    public Single Shift { get; set; }

  }

  public class TextureDefinitionShaderAnimation
  {

    [ScriptingProperty( "bending" )]
    public TextureDefinitionShaderAnimationBending Bending { get; set; }

    [ScriptingProperty( "blink" )]
    public TextureDefinitionShaderGradient Blink { get; set; }

    [ScriptingProperty( "enable" )]
    public Boolean Enable { get; set; }

    [ScriptingProperty( "gradient" )]
    public TextureDefinitionShaderGradient Gradient { get; set; }

    [ScriptingProperty( "noise" )]
    public TextureDefinitionAnimationNoise Noise { get; set; }

  }

  public class TextureDefinitionShaderReflection
  {

    [ScriptingProperty( "contrast" )]
    public Single Contrast { get; set; }

    [ScriptingProperty( "disturbance" )]
    public Single Disturbance { get; set; }

    [ScriptingProperty( "frameSkip" )]
    public Int32 FrameSkip { get; set; }

    [ScriptingProperty( "intensity" )]
    public Single Intensity { get; set; }

    [ScriptingProperty( "mipPower" )]
    public Single MipPower { get; set; }

    [ScriptingProperty( "useCubeMap" )]
    public Boolean UseCubeMap { get; set; }

  }

  public class TextureDefinitionShaderParallax
  {

    [ScriptingProperty( "scale" )]
    public Single Scale { get; set; }

    [ScriptingProperty( "shadow" )]
    public Boolean Shadow { get; set; }

  }

  public class TextureDefinitionShaderTranslucency
  {

    [ScriptingProperty( "blurWidth" )]
    public Single BlurWidth { get; set; }

    [ScriptingProperty( "firstColor" )]
    public S3DColor FirstColor { get; set; }

    [ScriptingProperty( "intensity" )]
    public Single Intensity { get; set; }

    [ScriptingProperty( "secondColor" )]
    public S3DColor SecondColor { get; set; }

    [ScriptingProperty( "width" )]
    public Single Width { get; set; }

  }

  public class TextureDefinitionShaderPerturbation
  {

    [ScriptingProperty( "enabled" )]
    public Boolean Enabled { get; set; }

    [ScriptingProperty( "first" )]
    public TextureDefinitionShaderPerturbationLayer FirstLayer { get; set; }

    [ScriptingProperty( "second" )]
    public TextureDefinitionShaderPerturbationLayer SecondLayer { get; set; }

  }

  public class TextureDefinitionShaderPerturbationLayer
  {

    [ScriptingProperty( "NM" )]
    public String NormalMap { get; set; }

    [ScriptingProperty( "scale" )]
    public Single Scale { get; set; }

    [ScriptingProperty( "time_scale" )]
    public Single TimeScale { get; set; }

    [ScriptingProperty( "wave_len" )]
    public Single WaveLength { get; set; }

  }

  public class TextureDefinitionShaderGlass
  {

    [ScriptingProperty( "filter" )]
    public Int32[] Filter { get; set; }

  }

  public class TextureDefinitionShaderSkin
  {

    [ScriptingProperty( "detail" )]
    public TextureDefinitionShaderDetailMap Detail { get; set; }

    [ScriptingProperty( "wrinkles" )]
    public TextureDefinitionShaderSkinWrinkles Wrinkles { get; set; }

  }

  public class TextureDefinitionShaderSkinWrinkles
  {

    [ScriptingProperty( "enable" )]
    public Boolean Enable { get; set; }

    [ScriptingProperty( "mask0_3" )]
    public String Mask0_3 { get; set; }

    [ScriptingProperty( "mask4_7" )]
    public String Mask4_7 { get; set; }

    [ScriptingProperty( "mask8_11" )]
    public String Mask8_11 { get; set; }

    [ScriptingProperty( "mask12_15" )]
    public String Mask12_15 { get; set; }

    [ScriptingProperty( "normalmap" )]
    public String NormalMap { get; set; }

  }

  public class TextureDefinitionShaderEyes
  {

    [ScriptingProperty( "glossiness" )]
    public Single Glossiness { get; set; }

    [ScriptingProperty( "intensity" )]
    public Single Intensity { get; set; }

    [ScriptingProperty( "nmScale" )]
    public Single NormalMapScale { get; set; }

    [ScriptingProperty( "x" )]
    public Single X { get; set; }

    [ScriptingProperty( "y" )]
    public Single Y { get; set; }

  }

  public class TextureDefinitionShaderFakeLight
  {

    [ScriptingProperty( "enabled" )]
    public Boolean Enabled { get; set; }

    [ScriptingProperty( "falloff_dist" )]
    public Single FalloffDistance { get; set; }

    [ScriptingProperty( "intensity" )]
    public Single Intensity { get; set; }

    [ScriptingProperty( "start_dist" )]
    public Single StartDistance { get; set; }

  }

  public class TextureDefinitionShaderTintByMask
  {

    [ScriptingProperty( "albedo" )]
    public S3DColor Albedo { get; set; }

    [ScriptingProperty( "carpaintMetallness" )]
    public S3DColor CarpaintMetallness { get; set; }

    [ScriptingProperty( "maskFromAlbedoAlpha" )]
    public Boolean MaskFromAlbedoAlpha { get; set; }

    [ScriptingProperty( "metallness" )]
    public S3DColor Metallness { get; set; }

  }

  public class TextureDefinitionShaderOverrideAffixScroll
  {

    [ScriptingProperty( "override" )]
    public Boolean OverrideEnabled { get; set; }

    [ScriptingProperty( "speedX" )]
    public Single SpeedX { get; set; }

    [ScriptingProperty( "speedY" )]
    public Single SpeedY { get; set; }

  }

  public class TextureDefinitionShaderDoubleSideLighting
  {

    [ScriptingProperty( "backLightTint" )]
    public S3DColor BackLightTint { get; set; }

    [ScriptingProperty( "use" )]
    public Boolean Use { get; set; }

    [ScriptingProperty( "viewDependence" )]
    public Single ViewDependence { get; set; }

  }

  public class TextureDefinitionShaderAnimationBending
  {

    [ScriptingProperty( "amplitude" )]
    public Single Amplitude { get; set; }

    [ScriptingProperty( "centerOffsetY" )]
    public Single CenterOffsetY { get; set; }

    [ScriptingProperty( "speed" )]
    public Single Speed { get; set; }

  }

  public class TextureDefinitionAnimationNoise
  {

    [ScriptingProperty( "amplitude" )]
    public Single Amplitude { get; set; }

    [ScriptingProperty( "speed" )]
    public Single Speed { get; set; }

  }

  public class TextureDefinitionDecay
  {

    [ScriptingProperty( "akill_falloff" )]
    public Single AlphaKillFalloff { get; set; }

    [ScriptingProperty( "akill_minval" )]
    public Single AlphaKillMinValue { get; set; }

    [ScriptingProperty( "akill_start_decay" )]
    public Single AlphaKillStartDecay { get; set; }

    [ScriptingProperty( "use_decay" )]
    public Boolean UseDecay { get; set; }

  }

  public class TextureDefinitionShaderMaps
  {

    [ScriptingProperty( "caustics" )]
    public String Caustics { get; set; }

    [ScriptingProperty( "debris" )]
    public String Debris { get; set; }

    [ScriptingProperty( "detail" )]
    public TextureDefinitionShaderDetailMap Detail { get; set; }

    [ScriptingProperty( "detail_nm" )]
    public TextureDefinitionShaderDetailMap DetailNormalMap { get; set; }

    [ScriptingProperty( "foam" )]
    public String Foam { get; set; }

    [ScriptingProperty( "globalMask" )]
    public String GlobalMask { get; set; }

    [ScriptingProperty( "shoreDataMap" )]
    public String ShoreDataMap { get; set; }

    [ScriptingProperty( "shoreMask" )]
    public String ShoreMask { get; set; }

    [ScriptingProperty( "tintMask" )]
    public String TintMask { get; set; }

  }

  public class TextureDefinitionShaderLayer
  {

    [ScriptingProperty( "add_bloom" )]
    public Single AddBloom { get; set; }

    [ScriptingProperty( "alphaSource" )]
    public String AlphaSource { get; set; }

    [ScriptingProperty( "blendMode" )]
    public String BlendMode { get; set; }

    [ScriptingProperty( "embossment_depth" )]
    public Single EmbossmentDepth { get; set; }

    [ScriptingProperty( "emissive" )]
    public Single Emissive { get; set; }

    [ScriptingProperty( "enable_embossment" )]
    public Boolean EnableEmbossment { get; set; }

    [ScriptingProperty( "fake_light_contrast" )]
    public Single FakeLightContrast { get; set; }

    [ScriptingProperty( "specular_intensity" )]
    public Single SpecularIntensity { get; set; }

    [ScriptingProperty( "specular_power" )]
    public Single SpecularPower { get; set; }

    [ScriptingProperty( "tex_diffuse" )]
    public String TextureDiffuse { get; set; }

    [ScriptingProperty( "tex_normalmap" )]
    public String TextureNormalMap { get; set; }

    [ScriptingProperty( "texture" )]
    public String Texture { get; set; }

    [ScriptingProperty( "tint" )]
    public S3DColor Tint { get; set; }

    [ScriptingProperty( "uv_scale_x" )]
    public Single UvScaleX { get; set; }

    [ScriptingProperty( "uv_scale_y" )]
    public Single UvScaleY { get; set; }

    [ScriptingProperty( "uv_scroll_speed_x" )]
    public Single UvScrollSpeedX { get; set; }

    [ScriptingProperty( "uv_scroll_speed_y" )]
    public Single UvScrollSpeedY { get; set; }

    [ScriptingProperty( "uv_source" )]
    public String UvSource { get; set; }

  }

  public class TextureDefinitionShaderColors
  {

    [ScriptingProperty( "ambientLightOverride" )]
    public S3DColor AmbientLightOverride { get; set; }

    [ScriptingProperty( "deepDepth" )]
    public Single DeepDepth { get; set; }

    [ScriptingProperty( "intensity" )]
    public Single Intensity { get; set; }

    [ScriptingProperty( "invertedNormal" )]
    public Boolean InvertedNormal { get; set; }

    [ScriptingProperty( "posShallow" )]
    public Single PosShallow { get; set; }

    [ScriptingProperty( "reflectionColorOverride" )]
    public S3DColor ReflectionColorOverride { get; set; }

    [ScriptingProperty( "skyAOOverride" )]
    public Single SkyAoOverride { get; set; }

    [ScriptingProperty( "ssrOverride" )]
    public Single SsrOverride { get; set; }

    [ScriptingProperty( "waterDeep" )]
    public Int32[] WaterDeep { get; set; }

    [ScriptingProperty( "waterDeepIntensity" )]
    public Single WaterDeepIntensity { get; set; }

    [ScriptingProperty( "waterScatter" )]
    public Int32[] WaterScatter { get; set; }

    [ScriptingProperty( "waterShallow" )]
    public Int32[] WaterShallow { get; set; }

  }

  public class TextureDefinititionShaderAdvanced
  {

    [ScriptingProperty( "causticsMultiplier" )]
    public Single CausticsMultiplier { get; set; }

    [ScriptingProperty( "fresnel" )]
    public TextureDefinitionShaderFresnel Fresnel { get; set; }

    [ScriptingProperty( "kSoft" )]
    public Single KSoft { get; set; }

  }

  public class TextureDefinitionShaderFresnel
  {

    [ScriptingProperty( "multiplier" )]
    public Single Multiplier { get; set; }

    [ScriptingProperty( "power" )]
    public Single Power { get; set; }

    [ScriptingProperty( "R0" )]
    public Single R0 { get; set; }

  }

  public class TextureDefinitionShaderRefraction
  {

    [ScriptingProperty( "opacityBias" )]
    public Single OpacityBias { get; set; }

    [ScriptingProperty( "opacityDepth" )]
    public Single OpacityDepth { get; set; }

    [ScriptingProperty( "tintBias" )]
    public Single TintBias { get; set; }

    [ScriptingProperty( "tintDepth" )]
    public Single TintDepth { get; set; }

  }

  public class TextureDefinitionShaderDiffuse
  {

    [ScriptingProperty( "multiplier" )]
    public Single Multiplier { get; set; }

    [ScriptingProperty( "shadowAmbient" )]
    public Single ShadowAmbient { get; set; }

  }

  public class TextureDefinitionShaderSpecular
  {

    [ScriptingProperty( "blinn" )]
    public PhongBlinn Blinn { get; set; }

    [ScriptingProperty( "phong" )]
    public PhongBlinn Phong { get; set; }

    [ScriptingProperty( "tint" )]
    public S3DColor Tint { get; set; }

  }

  public class PhongBlinn
  {

    [ScriptingProperty( "multiplier" )]
    public Single Multiplier { get; set; }

    [ScriptingProperty( "normalScale" )]
    public Single NormalScale { get; set; }

    [ScriptingProperty( "power" )]
    public Single Power { get; set; }

    [ScriptingProperty( "spotDepth" )]
    public Single SpotDepth { get; set; }

  }

  public class TextureDefinitionShaderFoam
  {

    [ScriptingProperty( "intensity" )]
    public Single Intensity { get; set; }

    [ScriptingProperty( "pos0" )]
    public Single Position0 { get; set; }

    [ScriptingProperty( "pos1" )]
    public Single Position1 { get; set; }

    [ScriptingProperty( "pos2" )]
    public Single Position2 { get; set; }

    [ScriptingProperty( "pos3" )]
    public Single Position3 { get; set; }

    [ScriptingProperty( "refluence" )]
    public TextureDefinitionShaderFoamRefluence Refluence { get; set; }

    [ScriptingProperty( "shift" )]
    public Single Shift { get; set; }

    [ScriptingProperty( "shore" )]
    public TextureDefinitionShaderFoamShore Shore { get; set; }

    [ScriptingProperty( "shoreFoamIntensity" )]
    public Single ShoreFoamIntensity { get; set; }

    [ScriptingProperty( "tiling" )]
    public Single Tiling { get; set; }

    [ScriptingProperty( "wave" )]
    public TextureDefinitionShaderFoamWave Wave { get; set; }

    [ScriptingProperty( "waveFoamIntensity" )]
    public Single WaveFoamIntensity { get; set; }

  }

  public class TextureDefinitionShaderFoamWave
  {

    [ScriptingProperty( "attenuation" )]
    public Single Attenuation { get; set; }

    [ScriptingProperty( "height" )]
    public Single Height { get; set; }

    [ScriptingProperty( "intensity" )]
    public Single Intensity { get; set; }

    [ScriptingProperty( "length" )]
    public Single Length { get; set; }

    [ScriptingProperty( "size" )]
    public Single Size { get; set; }

    [ScriptingProperty( "speed" )]
    public Single Speed { get; set; }

    [ScriptingProperty( "tail" )]
    public Int32 Tail { get; set; }

  }

  public class TextureDefinitionShaderFoamShore
  {

    [ScriptingProperty( "distribuance" )]
    public TextureDefinitionShaderFoamDistribuance Distribuance { get; set; }

    [ScriptingProperty( "intensity" )]
    public Single Intensity { get; set; }

    [ScriptingProperty( "pos0" )]
    public Single Position0 { get; set; }

    [ScriptingProperty( "pos1" )]
    public Single Position1 { get; set; }

    [ScriptingProperty( "pos2" )]
    public Single Position2 { get; set; }

    [ScriptingProperty( "pos3" )]
    public Single Position3 { get; set; }

    [ScriptingProperty( "size" )]
    public Single Size { get; set; }

    [ScriptingProperty( "tile" )]
    public Single Tile { get; set; }

  }

  public class TextureDefinitionShaderParams
  {

    [ScriptingProperty( "heightScale" )]
    public Single HeightScale { get; set; }

    [ScriptingProperty( "layerMultipliers" )]
    public TextureDefinitionShaderLayerMultipliers LayerMultipliers { get; set; }

    [ScriptingProperty( "normalScale" )]
    public Single NormalScale { get; set; }

    [ScriptingProperty( "simulateFlow" )]
    public TextureDefinitionShaderSimulateFlow SimulateFlow { get; set; }

    [ScriptingProperty( "simulateShore" )]
    public TextureDefinitionShaderSimulateShore SimulateShore { get; set; }

    [ScriptingProperty( "simulateWind" )]
    public Boolean SimulateWind { get; set; }

    [ScriptingProperty( "speed" )]
    public Single Speed { get; set; }

    [ScriptingProperty( "tilingScale" )]
    public Single TilingScale { get; set; }

    [ScriptingProperty( "wavesMaxDistance" )]
    public Single WavesMaxDistance { get; set; }

  }

  public class TextureDefinitionShaderLayerMultipliers
  {

    [ScriptingProperty( "layerScaleAtten" )]
    public Single LayerScaleAttenuation { get; set; }

    [ScriptingProperty( "layerWaveAtten" )]
    public Single LayerWaveAttenuation { get; set; }

  }

  public class TextureDefinitionShaderSimulateFlow
  {

    [ScriptingProperty( "disturbance" )]
    public Single Disturbance { get; set; }

    [ScriptingProperty( "enable" )]
    public Boolean Enable { get; set; }

    [ScriptingProperty( "speed" )]
    public Single Speed { get; set; }

  }

  public class TextureDefinitionShaderSimulateShore
  {

    [ScriptingProperty( "enable" )]
    public Boolean Enable { get; set; }

    [ScriptingProperty( "foam" )]
    public TextureDefinitionShaderFoam Foam { get; set; }

    [ScriptingProperty( "level" )]
    public Single Level { get; set; }

    [ScriptingProperty( "wave" )]
    public TextureDefinitionShaderFoamWave Wave { get; set; }

  }

  public class TextureDefinitionShaderAO
  {

    [ScriptingProperty( "intensity" )]
    public Single Intensity { get; set; }

    [ScriptingProperty( "occlusionAmount" )]
    public Single OcclusionAmount { get; set; }

    [ScriptingProperty( "vertexAmbientOcclusion" )]
    public Boolean VertexAmbientOcclusion { get; set; }

  }

  public class TextureDefinitionShaderFoamDistribuance
  {

    [ScriptingProperty( "amplitude" )]
    public Single Amplitude { get; set; }

    [ScriptingProperty( "freq" )]
    public Single Frequency { get; set; }

    [ScriptingProperty( "speed" )]
    public Single Speed { get; set; }

  }

  public class TextureDefinitionShaderFoamRefluence
  {

    [ScriptingProperty( "amplitude" )]
    public Single Amplitude { get; set; }

    [ScriptingProperty( "freq" )]
    public Single Frequency { get; set; }

    [ScriptingProperty( "posStart" )]
    public Single PositionStart { get; set; }

  }

}
