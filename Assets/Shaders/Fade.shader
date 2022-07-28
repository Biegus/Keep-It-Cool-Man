Shader "Custom/Fade"
{
	
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		 _OtherTex ("Other Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		 _V ("Value", Range(0.0,1)) = 1	

		 // *this shit doesn't work, dunno why* _StencilComp("Stencil Comparison", Float) = 3
		 _Stencil("Stencil ID", Float) = 0
		 _StencilOp("Stencil Operation", Float) = 0
		 _StencilWriteMask("Stencil Write Mask", Float) = 255
		 _StencilReadMask("Stencil Read Mask", Float) = 255
		 _ColorMask("Color Mask", Float) = 15
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent+20" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent-200" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Stencil
		 {
			 Ref[_Stencil]
			 Comp[_StencilComp]
			 Pass[_StencilOp]
			 ReadMask[_StencilReadMask]
			 WriteMask[_StencilWriteMask]
		 }
		  ColorMask[_ColorMask]

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
			};


			//base
			uniform  fixed4 _Color;
			uniform sampler2D _MainTex;
			uniform sampler2D _AlphaTex;
			uniform float _AlphaSplitEnabled;

			uniform sampler2D _OtherTex;
			uniform  float _V;
			
			

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}



			fixed4 SampleSpriteTexture (float2 uv, sampler2D smp)
            {
                fixed4 color = tex2D (smp, uv);
                return color;
            }
		
            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = SampleSpriteTexture ( IN.texcoord, _MainTex) * IN.color;
                fixed4 c2= SampleSpriteTexture (IN.texcoord, _OtherTex) * IN.color;
                fixed4 finalColor= lerp(c,c2,_V);
                
                finalColor.rgb*=finalColor.a;
                return  finalColor;
                
                
            }
		ENDCG
		}
	}
}