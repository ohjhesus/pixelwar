// Compiled shader for PC, Mac & Linux Standalone, uncompressed size: 1.1KB

// Skipping shader variants that would not be included into build of current scene.

Shader "Unlit/Transparent HealthBar" {
Properties {
 [PerRendererData]  _MainTex ("Sprite Texture", 2D) = "white" { }
 _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
}
SubShader { 
 LOD 100
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="TransparentCutout" "PreviewType"="Plane" "CanUseSpriteAtlas"="true" }


 // Stats for Vertex shader:
 //      opengl : 2 math, 2 texture, 1 branch
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="TransparentCutout" "PreviewType"="Plane" "CanUseSpriteAtlas"="true" }
  ZTest [unity_GUIZTestMode]
  ZWrite Off
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
  GpuProgramID 36113
Program "vp" {
SubProgram "opengl " {
// Stats: 2 math, 2 textures, 1 branches
"!!GLSL#version 120

#ifdef VERTEX

uniform vec4 _MainTex_ST;
varying vec2 xlv_TEXCOORD0;
void main ()
{
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = ((gl_MultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
}


#endif
#ifdef FRAGMENT
uniform sampler2D _MainTex;
uniform float _Cutoff;
varying vec2 xlv_TEXCOORD0;
void main ()
{
  vec4 tmpvar_1;
  tmpvar_1 = texture2D (_MainTex, xlv_TEXCOORD0);
  float x_2;
  x_2 = (tmpvar_1.w - _Cutoff);
  if ((x_2 < 0.0)) {
    discard;
  };
  gl_FragData[0] = tmpvar_1;
}


#endif
"
}
}
Program "fp" {
SubProgram "opengl " {
"!!GLSL"
}
}
 }
}
}