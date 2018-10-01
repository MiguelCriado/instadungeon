Shader "Sprites/Default Color Lerp - Occluded"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		[HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
		[PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
		_ColorLerp ("Color Lerp", Color) = (1, 1, 1, 1)
		[PerRendererData] _LerpAmount ("Lerp Amount", Range(0, 1)) = 0
		_OccludedColor ("Occluded Tint", Color) = (0, 0, 0, 0.5)
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
			Stencil
			{
				Ref 4
				Comp NotEqual
			}

		CGPROGRAM
			#pragma vertex SpriteVert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile_instancing
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnitySprites.cginc"

			fixed4 _ColorLerp;
			float _LerpAmount;

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 sampleColor = SampleSpriteTexture (IN.texcoord) * IN.color;
				fixed4 c = lerp(sampleColor, _ColorLerp, _LerpAmount);
				c.a = sampleColor.a;
				c.rgb *= c.a;
				return c;
			}

		ENDCG
		}

		Pass
		{
			Stencil
			{
				Ref 4
				Comp Equal
			}

		CGPROGRAM
			#pragma vertex SpriteVert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile_instancing
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnitySprites.cginc"

			fixed4 _ColorLerp;
			float _LerpAmount;
			fixed4 _OccludedColor;

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 sampleColor = SampleSpriteTexture (IN.texcoord);
				return _OccludedColor * sampleColor.a;
			}

		ENDCG
		}
	}
}
