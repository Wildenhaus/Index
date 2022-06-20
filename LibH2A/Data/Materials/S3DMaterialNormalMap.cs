﻿using Saber3D.Data.Scripting;

namespace Saber3D.Data.Materials
{

  public class S3DMaterialNormalMap
  {

    [ScriptingProperty( "end" )]
    public float End { get; set; }

    [ScriptingProperty( "falloff" )]
    public float Falloff { get; set; }

    [ScriptingProperty( "isVisible" )]
    public int IsVisible { get; set; }

    [ScriptingProperty( "scale" )]
    public float Scale { get; set; }

    [ScriptingProperty( "start" )]
    public float Start { get; set; }

    [ScriptingProperty( "textureName" )]
    public string TextureName { get; set; }

    [ScriptingProperty( "tilingU" )]
    public float TilingU { get; set; }

    [ScriptingProperty( "tilingV" )]
    public float TilingV { get; set; }

    [ScriptingProperty( "uvSetIdx" )]
    public int UvSetIndex { get; set; }

  }

}
